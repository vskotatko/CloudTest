using System;
using System.Collections.Generic;
using System.Text;

namespace CloudTest.DbAccess
{
  class DbModel
  {
    // parent-child table with ordering
    //   parentId long
    //   childId long
    //   sequenceId float ??? or consider linked list approach

    static DbModel currentConnection = null;

    static public void OpenConnection ()
    {
      if (currentConnection != null)
        throw new Exception ("DbModel: connection already open");
      currentConnection = new DbModel();

    }

    static public void CloseConnection ()
    {
      if (currentConnection != null)
      {
        currentConnection = null;
      }
    }
  }
}
