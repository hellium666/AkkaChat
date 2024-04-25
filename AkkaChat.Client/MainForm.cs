namespace AkkaChat.Client;

public sealed class MainForm : Form
{
    public delegate void AddGroupEvent(string groupName);
    public delegate void SendMessageEvent(string groupName, string message);

    public event AddGroupEvent? AddGroup;
    public event SendMessageEvent? SendMessage;
    

    private const int PadSize = 16;

    private readonly string _title;

    private readonly Button _joinGroupButton;
    private readonly TextBox _groupNameTextBox;
    private readonly ListBox _groupListBox;

    private ListBox? _currentChatListBox;
    private readonly SplitContainer _splitter;
    private readonly TextBox _chatMessageTextBox;
    private readonly Button _sendButton;

    public MainForm(string login)
    {
        _title = $"Akka.Chat - {login}";
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Text = $"{_title} [Connecting]";
        StartPosition = FormStartPosition.CenterScreen;

        _splitter = new SplitContainer();
        _splitter.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
        _splitter.Location = new Point(0, 0);
        _splitter.Size = ClientSize;
        _splitter.FixedPanel = FixedPanel.Panel1;
        _splitter.SplitterDistance = 250;
        Controls.Add(_splitter);

        _joinGroupButton = new Button();
        _joinGroupButton.Text = "Вступить";
        _joinGroupButton.Top = PadSize;
        _joinGroupButton.Left = _splitter.Panel1.Width - PadSize - _joinGroupButton.Width;
        _joinGroupButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _joinGroupButton.Enabled = false;
        _joinGroupButton.Click += OnAddGroupClick;
        _splitter.Panel1.Controls.Add(_joinGroupButton);

        _groupNameTextBox = new TextBox();
        _groupNameTextBox.PlaceholderText = "Имя группы";
        _groupNameTextBox.Left = PadSize;
        _groupNameTextBox.Top = PadSize;
        _groupNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
        _groupNameTextBox.Width = _joinGroupButton.Left - PadSize - PadSize;
        _groupNameTextBox.Enabled = false;
        _splitter.Panel1.Controls.Add(_groupNameTextBox);

        _groupListBox = new ListBox();
        _groupListBox.Top = _groupNameTextBox.Bottom + PadSize;
        _groupListBox.Left = PadSize;
        _groupListBox.Width = _splitter.Panel1.Width - PadSize - PadSize;
        _groupListBox.Height = _splitter.Panel1.Height - _groupListBox.Top - PadSize;
        _groupListBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
        _groupListBox.SelectedValueChanged += OnGroupSelected;
        _splitter.Panel1.Controls.Add(_groupListBox);

        _sendButton = new Button();
        _sendButton.Text = "Отправить";
        _sendButton.Top = PadSize;
        _sendButton.Left = _splitter.Panel2.Width - PadSize - _joinGroupButton.Width;
        _sendButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _sendButton.Enabled = false;
        _sendButton.Visible = false;
        _sendButton.Click += OnSendClick;
        _splitter.Panel2.Controls.Add(_sendButton);
        AcceptButton = _sendButton;
        
        _chatMessageTextBox = new TextBox();
        _chatMessageTextBox.PlaceholderText = "Сообщение";
        _chatMessageTextBox.Left = PadSize;
        _chatMessageTextBox.Top = PadSize;
        _chatMessageTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
        _chatMessageTextBox.Width = _sendButton.Left - PadSize - PadSize;
        _chatMessageTextBox.Visible = false;
        _splitter.Panel2.Controls.Add(_chatMessageTextBox);
    }

    private void OnSendClick(object? sender, EventArgs e)
    {
        if (_groupListBox.SelectedItem is not GroupChatState group)
            return;

        SendMessage?.Invoke(group.Name, _chatMessageTextBox.Text);
        _chatMessageTextBox.Text = string.Empty;
    }

    private void OnGroupSelected(object? sender, EventArgs e)
    {
        if (_currentChatListBox != null)
            _currentChatListBox.Visible = false;

        if (_groupListBox.SelectedItem is GroupChatState current)
        {
            current.ChatListBox.Visible = true;
            _currentChatListBox = current.ChatListBox;
            _chatMessageTextBox.Visible = true;
            _sendButton.Visible = true;
        }
        else
        {
            _chatMessageTextBox.Visible = false;
            _sendButton.Visible = false;
        }
    }

    private void OnAddGroupClick(object? sender, EventArgs e)
    {
        AddGroup?.Invoke(_groupNameTextBox.Text);
        _groupNameTextBox.Text = string.Empty;
    }

    public void OnJoinGroup(string groupName)
    {
        if (_groupListBox.Items.OfType<GroupChatState>().Any(item => item.Name == groupName))
            return;

        var chatListBox = new ListBox();
        chatListBox.Visible = false;
        chatListBox.Top = _chatMessageTextBox.Bottom + PadSize;
        chatListBox.Width = _splitter.Panel2.Width - PadSize - PadSize;
        chatListBox.Left = PadSize;
        chatListBox.Height = _splitter.Panel2.Height - PadSize - PadSize;

        _splitter.Panel2.Controls.Add(chatListBox);
        var state = new GroupChatState(groupName, chatListBox);
        _groupListBox.Items.Add(state);
    }

    public void OnLeftGroup(string groupName)
    {
        var item = _groupListBox.Items.OfType<GroupChatState>().FirstOrDefault(item => item.Name == groupName);
        if (item == null)
            return;

        _splitter.Panel2.Controls.Remove(item.ChatListBox);
        _groupListBox.Items.Remove(item);
    }

    public void OnConnectionClosed()
    {
        Close();
    }

    public void Connected()
    {
        Text = _title;
        EnableControls(true);
    }

    public void Reconnected()
    {
        Connected();
    }

    public void Reconnecting()
    {
        Text = $"{_title} [Reconnecting]";
        EnableControls(false);
    }

    private void EnableControls(bool enabled)
    {
        _joinGroupButton.Enabled = enabled;
        _groupNameTextBox.Enabled = enabled;
        _sendButton.Enabled = enabled;

    }

    public void ConnectionFailed(string message)
    {
        Text = $"{_title} [ConnectionFailed] {message}";
    }

    public void OnMessage(ChatMessage message)
    {
        var group = _groupListBox.Items.OfType<GroupChatState>().FirstOrDefault(m => m.Name == message.GroupName);
        group?.AddMessage(message);
    }
    
    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
        }

        base.Dispose(disposing);
    }
}