namespace DotnetProject2025.Models;

public class Account
{
    private string userID;
    private string userName;
    private string email;
    private string password;

       
    public Account()
    {
    }

       
    public Account(string userID, string userName, string email, string password)
    {
        this.userID = userID;
        this.userName = userName;
        this.email = email;
        this.password = password;
    }

   
    public string  UserID
    {
        get { return userID; }
        set { userID = value; }
    }

    public string UserName
    {
        get { return userName; }
        set { userName = value; }
    }

    public string Email
    {
        get { return email; }
        set { email = value; }
    }

    
    public string Password
    {
        get { return password; }
        set { password = value; }
    }

       
    public override string ToString()
    {
        return $"Account: [UserID={userID}, UserName={userName}, Email={email}, Password={password}]";
    }

}