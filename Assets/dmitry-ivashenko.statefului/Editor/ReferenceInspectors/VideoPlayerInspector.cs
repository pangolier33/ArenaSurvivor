using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class VideoPlayerInspector : ReadOnlyTwoColumnsInspector
    {
        protected override Type RoleType => typeof(VideoPlayerRoleAttribute);
        protected override string FirstFieldName => nameof(VideoPlayerReference.Role);
#if VIDEO_MODULE
        protected override string SecondFieldName => nameof(VideoPlayerReference.VideoPlayer);
#else
	    protected override string SecondFieldName => "";
#endif
        public VideoPlayerInspector(SerializedObject serializedObject) 
            : base(serializedObject, nameof(StatefulComponent.VideoPlayers))
        {
        }

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetVideoPlayer(VideoPlayerRole.{name});\n";
        }
    }
}