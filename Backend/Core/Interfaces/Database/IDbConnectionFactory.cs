using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Database
{
    /// <summary>
    /// interface kết nối DB
    /// </summary>
    /// Created by Phuong 25/02/2026
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
