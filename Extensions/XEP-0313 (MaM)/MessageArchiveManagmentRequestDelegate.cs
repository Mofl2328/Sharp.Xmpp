namespace Sharp.Xmpp.Extensions
{
    /// <summary>
    /// Invoked when a CustomIqRequest is made.
    /// </summary>
    /// <param name="jid">The jid</param>
    /// <param name="str">The serialised data stream</param>
    /// <returns>The serialised anwser string</returns>
    public delegate void MessageArchiveManagmentRequestDelegate(string id, string str);
}