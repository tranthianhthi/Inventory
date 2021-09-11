using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACFC.Common.Models;

namespace ACFC.Common.Interfaces
{
    public interface IStore
    {
        /// <summary>
        /// Finds the stores.
        /// </summary>
        /// <returns></returns>
        List<Store> FindStores();

        /// <summary>
        /// Finds the stores.
        /// </summary>
        /// <param name="storeCode">The store code.</param>
        /// <returns></returns>
        List<Store> FindStores(string storeCode);

        /// <summary>
        /// Finds the store.
        /// </summary>
        /// <param name="storeNo">The store no.</param>
        /// <returns></returns>
        Store FindStore(int storeNo);

    }
}
