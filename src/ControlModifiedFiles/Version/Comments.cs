using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class Comments
    {
        internal FileSubscriber Subscriber { get; }
        internal FileInfo FileInfo { get; }

        public Comments(FileSubscriber subscriber, FileInfo fileInfo)
        {
            Subscriber = subscriber;
            FileInfo = fileInfo;
        }

        internal void UpdateCommentFile(string textComment)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Subscriber.DirectoryVersion);

            string pathFileComment = Path.Combine(directoryInfo.FullName, "comments.js");

            FileInfo fileInfoComment = new FileInfo(pathFileComment);

            string dataFile = string.Empty;
            if (fileInfoComment.Exists)
            {
                using (StreamReader reader = new StreamReader(fileInfoComment.FullName))
                {
                    dataFile = reader.ReadToEnd();
                }
            }

            List<CommentsVersion> comments = null;
            if (string.IsNullOrWhiteSpace(dataFile))
                comments = new List<CommentsVersion>();
            else
                comments = new Json<CommentsVersion>().Deserialize(dataFile);
                         
            comments.Add(new CommentsVersion()
            {
                Comment = textComment,
                FileName = FileInfo.Name,
                DateTime = DateTime.Now
            });

            dataFile = new Json<CommentsVersion>().Serialize(comments);

            using (StreamWriter stream = new StreamWriter(fileInfoComment.FullName, false))
            {
                stream.WriteLine(dataFile);
                stream.Flush();
            }
        }

    }
}
