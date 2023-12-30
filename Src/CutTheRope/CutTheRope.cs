using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;

namespace GameManager
{
   [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
   [DebuggerNonUserCode]
   [CompilerGenerated]
    internal class GameManager
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (ReferenceEquals(resourceMan, null))
                {
                    ResourceManager resourceManager = default;
                       // new ResourceManager();//("CutTheRope.CutTheRope", typeof(CutTheRope).Assembly);
                    resourceMan = resourceManager;
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string FakeString
        {
            get
            {
                return default;//ResourceManager.GetString("FakeString", resourceCulture);
            }
        }

        internal GameManager()
        {
        }
    }
}
