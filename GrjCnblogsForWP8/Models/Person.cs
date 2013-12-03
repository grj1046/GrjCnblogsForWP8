using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class Person
    {
        public Person() { }

        public Person(string persionalPage, string avatarImgSrc, string nickName)
        {
            this.PersionalPage = persionalPage;
            this.AvatarImgSrc = avatarImgSrc;
            this.NickName = nickName;
        }

        public string NickName { get; set; }
        public string AvatarImgSrc { get; set; }
        public string PersionalPage { get; set; }
    }
}
