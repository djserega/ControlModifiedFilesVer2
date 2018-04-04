using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace ControlModifiedFiles
{
    internal class Json<T> where T : class
    {
        internal List<T> Deserialize(string inputText)
        {
            var serializer = new JavaScriptSerializer
            {
                MaxJsonLength = inputText.Length
            };

            var objectRequest = serializer.Deserialize<List<T>>(inputText);

            return objectRequest;
        }

        internal string Serialize(List<T> obj)
        {
            var serializer = new JavaScriptSerializer();

            return serializer.Serialize(obj);
        }

    }
}
