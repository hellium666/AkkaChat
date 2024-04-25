namespace AkkaChat.Client;

public sealed class LoginForm : Form
{
    private const int PadSize = 16;

    private readonly TextBox _loginTextBox;
    private readonly Button _loginButton;

    public string Login { get; private set; }
    
    public LoginForm()
    {
        Login = string.Empty;
        
        AutoScaleMode = AutoScaleMode.Font;

        Text = "Введите логин";

        _loginTextBox = new TextBox();
        _loginTextBox.Top = PadSize;
        _loginTextBox.Left = PadSize;
        _loginTextBox.Width = 200;
        _loginTextBox.PlaceholderText = "Введите логин";
        _loginTextBox.TextChanged += OnLoginTextChanged;
        Controls.Add(_loginTextBox);

        _loginButton = new Button();
        _loginButton.Top = PadSize;
        _loginButton.Left = _loginTextBox.Right + PadSize;
        _loginButton.Enabled = false;
        _loginButton.Text = "Войти";
        _loginButton.Click += OnLoginButtonClick;
        Controls.Add(_loginButton);
        
        ClientSize = new Size(_loginButton.Right + PadSize, _loginButton.Bottom + PadSize);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        AcceptButton = _loginButton;
    }
    
    private bool LoginIsValid()
    {
        return _loginTextBox.Text.Length > 2;
    }

    private void OnLoginTextChanged(object? sender, EventArgs e)
    {
        var isValid = LoginIsValid();
        _loginButton.Enabled = isValid;
        Login = isValid ? _loginTextBox.Text : string.Empty;
    }

    private void OnLoginButtonClick(object? sender, EventArgs e)
    {
        if (LoginIsValid())
            Close();
    }
    
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loginButton.Dispose();
            _loginTextBox.Dispose();
        }

        base.Dispose(disposing);
    }
}