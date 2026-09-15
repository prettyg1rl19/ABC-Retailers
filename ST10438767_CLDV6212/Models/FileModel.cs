namespace ST10438767_CLDV6212.Models
{
    public class FileModel
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                    return $"{Size / 1024 / 1024} MB";
                if (Size >= 1024)
                    return $"{Size / 1024} KB";
                return $"{Size} Bytes";
            }
        }
    }
}

/*IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share!
[video online] Available at:<https://www.youtube.com/watch?v=A-mVVL88oEg&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=7> [Accessed 20 August 2025]. 
*/
