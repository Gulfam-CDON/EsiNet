﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiCompositeFragmentExecutor
    {
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiCompositeFragmentExecutor(EsiFragmentExecutor fragmentExecutor)
        {
            _fragmentExecutor = fragmentExecutor ?? throw new ArgumentNullException(nameof(fragmentExecutor));
        }

        public async Task<IEnumerable<string>> Execute(EsiCompositeFragment fragment)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            var tasks = fragment.Fragments
                .Select(_fragmentExecutor.Execute);
            var results = await Task.WhenAll(tasks);

            return results.SelectMany(s => s);
        }
    }
}