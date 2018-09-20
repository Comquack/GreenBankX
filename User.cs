using System;

public class User
{
    public string First { get; set; }
    public string Last { get; set; }
    public int id { get; }
    public User(string first, string last)
	{
        this.First = first;
        this.Last = last;
	}
}
