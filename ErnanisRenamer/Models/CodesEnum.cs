using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnanisRenamer.Models
{
    public enum ProcessCode
    {
        ON_INIT = 0,
        ON_SELECT_BY_FOLDER,
        ON_SELECT_BY_MULTIPLE_FILES,
        ON_RENAME_INIT,
        ON_RENAME_SUCCESS,
        ON_UPDATE_FILTER
    }
    public enum ErrorCode
    {
        NO_ERROR = 0,
        NO_FOLDER_EXIST

    }
    
}