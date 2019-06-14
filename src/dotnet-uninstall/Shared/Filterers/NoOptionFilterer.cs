﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class NoOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<Bundle> Filter(IEnumerable<string> argValue, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            if ((int)typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (typeSelection != BundleType.Sdk || typeSelection != BundleType.Runtime)
            {
                throw new BundleTypeNotSpecifiedException();
            }

            var bundlesWithSelectedType = bundles
                .Where(bundle => (typeSelection & bundle.Version.Type) > 0);

            var anyMissing = argValue
                .Where(next => !bundles
                    .Select(bundle => bundle.Version.ToString())
                    .ToList()
                    .Contains(next));

            if (anyMissing.Count() > 0)
            {
                throw new SpecifiedVersionNotFoundException(anyMissing.First());
            }

            var specifiedVersions = bundlesWithSelectedType
                .Select(bundle => bundle.Version)
                .Where(version => argValue.Contains(version.ToString()))
                .OrderBy(version => version);

            return bundles
                .Where(bundle => specifiedVersions.Contains(bundle.Version));
        }
    }
}
