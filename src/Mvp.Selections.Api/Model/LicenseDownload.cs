using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvp.Selections.Api.Model
{
    public class LicenseDownload
    {
        public required string XmlContent { get; set; }

        public required string FileName { get; set; }
    }
}
