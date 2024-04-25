namespace AkkaChat.Client;

internal class GroupChatState
{
    public GroupChatState(string name, ListBox chatListBox)
    {
        Name = name;
        ChatListBox = chatListBox;
    }

    public string Name { get; }
    public ListBox ChatListBox { get; }


    public void AddMessage(ChatMessage message)
    {
        ChatListBox.Items.Add($"{message.Timestamp:s} - {message.From} - {message.Message}");
        ChatListBox.TopIndex = ChatListBox.Items.Count - 1;
    }

    public override string ToString()
    {
        return Name;
    }
}