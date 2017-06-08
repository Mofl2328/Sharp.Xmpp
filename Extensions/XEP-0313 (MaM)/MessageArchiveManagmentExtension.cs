using Sharp.Xmpp.Core;
using Sharp.Xmpp.Im;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Sharp.Xmpp.Extensions
{
    /// <summary>
    /// Implements Mechanism for providing MaM support
    /// </summary>
    internal class MessageArchiveManagment : XmppExtension
    {
        /// <summary>
        /// A reference to the 'Entity Capabilities' extension instance.
        /// 
        /// Not yet supported
        /// </summary>
        private EntityCapabilities ecapa;

        /// <summary>
        /// An enumerable collection of XMPP namespaces the extension implements.
        /// </summary>
        /// <remarks>This is used for compiling the list of supported extensions
        /// advertised by the 'Service Discovery' extension.</remarks>
        public override IEnumerable<string> Namespaces
        {
            get
            {
                return new string[] { "urn:xmpp:mam:1"  };
            }
        }

        /// <summary>
        /// The named constant of the Extension enumeration that corresponds to this
        /// extension.
        /// </summary>
        public override Extension Xep
        {
            get
            {
                return Extension.MessageArchiveManagment;
            }
        }

        /// <summary>
        /// Invoked after all extensions have been loaded.
        /// </summary>
        public override void Initialize()
        {
            ecapa = im.GetExtension<EntityCapabilities>();
        }


        /// <summary>
        /// Requests the XMPP entity with the specified JID a GET command.
        /// When the Result is received and it not not an error
        /// if fires the callback function
        /// </summary>
        /// <param name="jid">The JID of the XMPP entity to get.</param>
        /// <exception cref="ArgumentNullException">The jid parameter
        /// is null.</exception>
        /// <exception cref="NotSupportedException">The XMPP entity with
        /// the specified JID does not support the 'Ping' XMPP extension.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public void RequestCustomIqAsync(Jid jid, int max, MessageArchiveManagmentRequestDelegate callback, string before = null, string after = null)
        {
            jid.ThrowIfNull("jid");

            /*/First check if the Jid entity supports the namespace
            if (ecapa.Supports(jid, Extension.CustomIqExtension))
            {
                throw new NotSupportedException("The XMPP entity does not support the " +
                    "'CustomIqExtension' extension.");
            }//*/
            var xml = Xml.Element("query", "urn:xmpp:mam:1");
            var xmlParam = Xml.Element("set", "http://jabber.org/protocol/rsm");
            if ( max > 0 ) xmlParam.Child(Xml.Element("max").Text(max.ToString()));
            if (before == null)
                xmlParam.Child(Xml.Element("before"));
            else
                xmlParam.Child(Xml.Element("before").Text(before));
            if (after != null)
                xmlParam.Child(Xml.Element("after").Text(after));
            xml.Child(xmlParam);

            Debug.WriteLine(xml);

            //The Request is Async
            im.IqRequestAsync(IqType.Set, jid, im.Jid, xml, null, (id, iq) =>
            {
                Debug.WriteLine(iq);

                //For any reply we execute the callback
                if (iq.Type == IqType.Error)
                    throw Util.ExceptionFromError(iq, "Could not Send Object to XMPP entity.");
                if (iq.Type == IqType.Result)
                {
                    try
                    {
                        //An empty response means the message was received
                        if (callback != null)
                        {
                            callback.Invoke(id, iq.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Not correctly formated response to RequestCustomIqAsync" + e.StackTrace + e.ToString());
                        throw Util.ExceptionFromError(iq, "Not correctly formated response to RequestCustomIqAsync, " + e.Message);
                    }
                }
            });
        }



        /// <summary>
        /// Initializes a new instance of the CustomIq class.
        /// </summary>
        /// <param name="im">A reference to the XmppIm instance on whose behalf this
        /// instance is created.</param>
        public MessageArchiveManagment(XmppIm im)
            : base(im)
        {
        }
    }
}