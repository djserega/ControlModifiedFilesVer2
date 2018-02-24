using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ControlModifiedFiles
{
    public class FileSubscriber
    {
        [Column("Выбрано", IsOnlyRead = false)]
        public bool Checked { get; set; }

        [Column("Путь к файлу", Visible = false)]
        public string Path { get; set; }

        [Column("Имя файла", Visible = false)]
        public string FileName { get; set; }

        [Column("Размер (byte)", Visible = false)]
        public ulong Size { get; set; }

        [Column("Размер", SortMemberPath = "Size")]
        public string SizeString { get; set; }

        [Column("№ версии")]
        public int Version { get; set; }

        [Column("Дата изменения")]
        public DateTime DateMaxVersion { get; set; }

        [Column("Каталог версий", Visible = false)]
        public string DirectoryVersion { get; set; }

    }
}
