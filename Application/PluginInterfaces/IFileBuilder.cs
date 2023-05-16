using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PluginInterfaces
{
    public interface IFileBuilder
    {
        byte[]  BuildExcelFile(DataTable dataTable);
        byte[] BuildCSVFile<T>(IEnumerable<T> source, string exportDataType);
    }
}
