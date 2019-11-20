using DG.DemiEditor;
using DG.DOTweenEditor.Core;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DG.DOTweenEditor
{
	[CustomEditor(typeof(DOTweenPath))]
	public class DOTweenPathInspector : ABSAnimationInspector
	{
		private readonly Color _wpColor = Color.white;

		private readonly Color _arrowsColor = new Color(1f, 1f, 1f, 0.85f);

		private readonly Color _wpColorEnd = Color.red;

		private DOTweenPath _src;

		private readonly List<WpHandle> _wpsByDepth = new List<WpHandle>();

		private int _minHandleControlId;

		private int _maxHandleControlId;

		private int _selectedWpIndex = -1;

		private int _lastSelectedWpIndex = -1;

		private int _lastCreatedWpIndex = -1;

		private bool _changed;

		private Vector3 _lastSceneViewCamPosition;

		private Quaternion _lastSceneViewCamRotation;

		private bool _isDragging;

		private bool _reselectAfterDrag;

		private bool _sceneCamStored;

		private bool _refreshAfterEnable;

		private Camera _fooSceneCam;

		private Transform _fooSceneCamTrans;

		private ReorderableList _wpsList;

		public bool updater;

		private Camera _sceneCam
		{
			get
			{
				if ((UnityEngine.Object)this._fooSceneCam == (UnityEngine.Object)null)
				{
					SceneView currentDrawingSceneView = SceneView.currentDrawingSceneView;
					if ((UnityEngine.Object)currentDrawingSceneView == (UnityEngine.Object)null)
					{
						return null;
					}
					this._fooSceneCam = currentDrawingSceneView.camera;
				}
				return this._fooSceneCam;
			}
		}

		private Transform _sceneCamTrans
		{
			get
			{
				if ((UnityEngine.Object)this._fooSceneCamTrans == (UnityEngine.Object)null)
				{
					if ((UnityEngine.Object)this._sceneCam == (UnityEngine.Object)null)
					{
						return null;
					}
					this._fooSceneCamTrans = this._sceneCam.transform;
				}
				return this._fooSceneCamTrans;
			}
		}

		private void OnEnable()
		{
			this._src = (base.target as DOTweenPath);
			this.StoreSceneCamData();
			if (this._src.path == null)
			{
				this.ResetPath(RepaintMode.None);
			}
			base.onStartProperty = base.serializedObject.FindProperty("onStart");
			base.onPlayProperty = base.serializedObject.FindProperty("onPlay");
			base.onUpdateProperty = base.serializedObject.FindProperty("onUpdate");
			base.onStepCompleteProperty = base.serializedObject.FindProperty("onStepComplete");
			base.onCompleteProperty = base.serializedObject.FindProperty("onComplete");
			base.onTweenCreatedProperty = base.serializedObject.FindProperty("onTweenCreated");
			this._refreshAfterEnable = true;
		}

		public override void OnInspectorGUI()
		{
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0991: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
			base.OnInspectorGUI();
			EditorGUIUtils.SetGUIStyles(null);
			GUILayout.Space(3f);
			EditorGUIUtils.InspectorLogo();
			if (Application.isPlaying)
			{
				GUILayout.Space(8f);
				GUILayout.Label("Path Editor disabled while in play mode", EditorGUIUtils.wordWrapLabelStyle);
				GUILayout.Space(10f);
			}
			else
			{
				if (this._refreshAfterEnable)
				{
					this._refreshAfterEnable = false;
					if (this._src.path == null)
					{
						this.ResetPath(RepaintMode.None);
					}
					else
					{
						this.RefreshPath(RepaintMode.Scene, true);
					}
					this._wpsList = new ReorderableList(this._src.wps, typeof(Vector3), true, true, true, true);
					this._wpsList.drawHeaderCallback = delegate(Rect rect)
					{
						EditorGUI.LabelField(rect, "Path Waypoints");
					};
					this._wpsList.onReorderCallback = delegate
					{
						this.RefreshPath(RepaintMode.Scene, true);
					};
					this._wpsList.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
					{
						Rect position = new Rect(rect.xMin, rect.yMin, 23f, rect.height);
						Rect position2 = new Rect(position.xMax, position.yMin, rect.width - 23f, position.height);
						GUI.Label(position, (index + 1).ToString());
						this._src.wps[index] = EditorGUI.Vector3Field(position2, "", this._src.wps[index]);
					};
				}
				bool flag = false;
				Undo.RecordObject(this._src, "DOTween Path");
				if (!((UnityEngine.Object)((Component)this._src).GetComponent<DOTweenVisualManager>() != (UnityEngine.Object)null) && this._src.inspectorMode != DOTweenInspectorMode.InfoAndWaypointsOnly)
				{
					if (GUILayout.Button(new GUIContent("Add Manager", "Adds a manager component which allows you to choose additional options for this gameObject")))
					{
						this._src.gameObject.AddComponent<DOTweenVisualManager>();
					}
					GUILayout.Space(4f);
				}
				AnimationInspectorGUI.StickyTitle("Scene View Commands");
				DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
				GUILayout.Label("➲ SHIFT + " + (EditorUtils.isOSXEditor ? "CMD" : "CTRL") + ": add a waypoint\n➲ SHIFT + ALT: remove a waypoint");
				DeGUILayout.EndVBox();
				AnimationInspectorGUI.StickyTitle("Info");
				DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
				GUILayout.Label("Path Length: " + ((this._src.path == null) ? "-" : this._src.path.length.ToString()));
				DeGUILayout.EndVBox();
				if (this._src.inspectorMode != DOTweenInspectorMode.InfoAndWaypointsOnly)
				{
					AnimationInspectorGUI.StickyTitle("Tween Options");
					GUILayout.BeginHorizontal();
					this._src.autoPlay = DeGUILayout.ToggleButton(this._src.autoPlay, new GUIContent("AutoPlay", "If selected, the tween will play automatically"), DeGUI.styles.button.tool, new GUILayoutOption[0]);
					this._src.autoKill = DeGUILayout.ToggleButton(this._src.autoKill, new GUIContent("AutoKill", "If selected, the tween will be killed when it completes, and won't be reusable"), DeGUI.styles.button.tool, new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
					DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
					GUILayout.BeginHorizontal();
					this._src.duration = EditorGUILayout.FloatField("Duration", this._src.duration);
					if (this._src.duration < 0f)
					{
						this._src.duration = 0f;
					}
					this._src.isSpeedBased = DeGUILayout.ToggleButton(this._src.isSpeedBased, new GUIContent("SpeedBased", "If selected, the duration will count as units/degree x second"), DeGUI.styles.button.tool, new GUILayoutOption[1]
					{
						GUILayout.Width(75f)
					});
					GUILayout.EndHorizontal();
					this._src.delay = EditorGUILayout.FloatField("Delay", this._src.delay);
					if (this._src.delay < 0f)
					{
						this._src.delay = 0f;
					}
					this._src.easeType = EditorGUIUtils.FilteredEasePopup(this._src.easeType);
					if (this._src.easeType == Ease.INTERNAL_Custom)
					{
						this._src.easeCurve = EditorGUILayout.CurveField("   Ease Curve", this._src.easeCurve);
					}
					this._src.loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), this._src.loops);
					if (this._src.loops < -1)
					{
						this._src.loops = -1;
					}
					if (this._src.loops > 1 || this._src.loops == -1)
					{
						this._src.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", (Enum)(object)this._src.loopType);
					}
					this._src.id = EditorGUILayout.TextField("ID", this._src.id);
					this._src.updateType = (UpdateType)EditorGUILayout.EnumPopup("Update Type", (Enum)(object)this._src.updateType);
					DeGUILayout.EndVBox();
					AnimationInspectorGUI.StickyTitle("Path Tween Options");
					DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
					PathType pathType = this._src.pathType;
					this._src.pathType = (PathType)EditorGUILayout.EnumPopup("Path Type", (Enum)(object)this._src.pathType);
					if (pathType != this._src.pathType)
					{
						flag = true;
					}
					if (this._src.pathType != 0)
					{
						this._src.pathResolution = EditorGUILayout.IntSlider("   Path resolution", this._src.pathResolution, 2, 20);
					}
					bool isClosedPath = this._src.isClosedPath;
					this._src.isClosedPath = EditorGUILayout.Toggle("Close Path", this._src.isClosedPath);
					if (isClosedPath != this._src.isClosedPath)
					{
						flag = true;
					}
					this._src.isLocal = EditorGUILayout.Toggle(new GUIContent("Local Movement", "If checked, the path will tween the localPosition (instead than the position) of its target"), this._src.isLocal);
					this._src.pathMode = (PathMode)EditorGUILayout.EnumPopup("Path Mode", (Enum)(object)this._src.pathMode);
					this._src.lockRotation = (AxisConstraint)EditorGUILayout.EnumPopup("Lock Rotation", (Enum)(object)this._src.lockRotation);
					this._src.orientType = (OrientType)EditorGUILayout.EnumPopup("Orientation", (Enum)(object)this._src.orientType);
					if (this._src.orientType != 0)
					{
						switch (this._src.orientType)
						{
						case OrientType.LookAtTransform:
							this._src.lookAtTransform = (EditorGUILayout.ObjectField("   LookAt Target", this._src.lookAtTransform, typeof(Transform), true) as Transform);
							break;
						case OrientType.LookAtPosition:
							this._src.lookAtPosition = EditorGUILayout.Vector3Field("   LookAt Position", this._src.lookAtPosition);
							break;
						case OrientType.ToPath:
							this._src.lookAhead = EditorGUILayout.Slider("   LookAhead", this._src.lookAhead, 0f, 1f);
							break;
						}
					}
					DeGUILayout.EndVBox();
					AnimationInspectorGUI.StickyTitle("Path Editor Options");
					DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
					this._src.relative = EditorGUILayout.Toggle(new GUIContent("Relative", "If toggled, the whole path moves with the target"), this._src.relative);
					this._src.pathColor = EditorGUILayout.ColorField("Color", this._src.pathColor);
					this._src.showIndexes = EditorGUILayout.Toggle("Show Indexes", this._src.showIndexes);
					this._src.showWpLength = EditorGUILayout.Toggle("Show WPs Lengths", this._src.showWpLength);
					this._src.livePreview = EditorGUILayout.Toggle("Live Preview", this._src.livePreview);
					this._src.handlesType = (HandlesType)EditorGUILayout.EnumPopup("Handles Type", (Enum)(object)this._src.handlesType);
					this._src.handlesDrawMode = (HandlesDrawMode)EditorGUILayout.EnumPopup("Handles Mode", (Enum)(object)this._src.handlesDrawMode);
					if (this._src.handlesDrawMode == HandlesDrawMode.Perspective)
					{
						this._src.perspectiveHandleSize = EditorGUILayout.FloatField("   Handle Size", this._src.perspectiveHandleSize);
					}
					DeGUILayout.EndVBox();
					GUILayout.Space(6f);
					if (GUILayout.Button("Reset Path"))
					{
						this.ResetPath(RepaintMode.SceneAndInspector);
					}
					AnimationInspectorGUI.AnimationEvents(this, this._src);
				}
				else
				{
					GUILayout.Space(6f);
					if (GUILayout.Button("Reset Path"))
					{
						this.ResetPath(RepaintMode.SceneAndInspector);
					}
				}
				GUILayout.Space(10f);
				DeGUILayout.BeginToolbar(new GUILayoutOption[0]);
				this._src.wpsDropdown = DeGUILayout.ToolbarFoldoutButton(this._src.wpsDropdown, "Waypoints", false, false);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent("Copy to clipboard", "Copies the current waypoints to clipboard, as an array ready to be pasted in your code"), DeGUI.styles.button.tool))
				{
					this.CopyWaypointsToClipboard();
				}
				DeGUILayout.EndToolbar();
				if (this._src.wpsDropdown)
				{
					DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
					bool changed = GUI.changed;
					this._wpsList.DoLayoutList();
					if (!changed && GUI.changed)
					{
						flag = true;
					}
					DeGUILayout.EndVBox();
				}
				else
				{
					GUILayout.Space(5f);
				}
				if (flag)
				{
					this.RefreshPath(RepaintMode.Scene, false);
				}
				else if (GUI.changed)
				{
					EditorUtility.SetDirty(this._src);
					this.DORepaint(RepaintMode.Scene, false);
				}
			}
		}

		private void OnSceneGUI()
		{
			if (!Application.isPlaying)
			{
				this.StoreSceneCamData();
				if (this._src.gameObject.activeInHierarchy && this._sceneCamStored)
				{
					if (this._wpsByDepth.Count != this._src.wps.Count)
					{
						this.FillWpIndexByDepth();
					}
					EditorGUIUtils.SetGUIStyles(null);
					Event current = Event.current;
					Undo.RecordObject(this._src, "DOTween Path");
					if (current.type == EventType.MouseDown)
					{
						if (current.shift)
						{
							if (EditorGUI.actionKey)
							{
								Vector3 vector = (this._lastCreatedWpIndex != -1) ? this._src.wps[this._lastCreatedWpIndex] : ((this._selectedWpIndex != -1) ? this._src.wps[this._selectedWpIndex] : ((this._lastSelectedWpIndex != -1) ? this._src.wps[this._lastSelectedWpIndex] : this._src.transform.position));
								Matrix4x4 worldToCameraMatrix = this._sceneCam.worldToCameraMatrix;
								float z = 0f - (worldToCameraMatrix.m20 * vector.x + worldToCameraMatrix.m21 * vector.y + worldToCameraMatrix.m22 * vector.z + worldToCameraMatrix.m23);
								Camera sceneCam = this._sceneCam;
								float x = current.mousePosition.x;
								Rect pixelRect = this._sceneCam.pixelRect;
								float x2 = x / pixelRect.width;
								float y = current.mousePosition.y;
								pixelRect = this._sceneCam.pixelRect;
								Vector3 item = sceneCam.ViewportToWorldPoint(new Vector3(x2, 1f - y / pixelRect.height, z));
								if (this._selectedWpIndex != -1 && this._selectedWpIndex < this._src.wps.Count - 1)
								{
									this._src.wps.Insert(this._selectedWpIndex + 1, item);
									this._lastCreatedWpIndex = this._selectedWpIndex + 1;
									this._selectedWpIndex = this._lastCreatedWpIndex;
								}
								else
								{
									this._src.wps.Add(item);
									this._lastCreatedWpIndex = this._src.wps.Count - 1;
									this._selectedWpIndex = this._lastCreatedWpIndex;
								}
								this.RefreshPath(RepaintMode.Scene, true);
								return;
							}
							if (current.alt && this._src.wps.Count > 1)
							{
								this.FindSelectedWaypointIndex();
								if (this._selectedWpIndex != -1)
								{
									this._src.wps.RemoveAt(this._selectedWpIndex);
									this.ResetIndexes();
									this.RefreshPath(RepaintMode.Scene, true);
									return;
								}
							}
						}
						this.FindSelectedWaypointIndex();
					}
					if (this._src.wps.Count >= 1)
					{
						if (current.type == EventType.MouseDrag)
						{
							this._isDragging = true;
							if (this._src.livePreview)
							{
								bool flag = this.CheckTargetMove();
								if (this._selectedWpIndex != -1)
								{
									flag = true;
								}
								if (flag)
								{
									this.RefreshPath(RepaintMode.Scene, false);
								}
							}
						}
						else if (this._isDragging && current.rawType == EventType.MouseUp)
						{
							if (this._isDragging && this._selectedWpIndex != -1)
							{
								this._reselectAfterDrag = true;
							}
							this._isDragging = false;
							if (this._selectedWpIndex != -1 || this.CheckTargetMove())
							{
								EditorUtility.SetDirty(this._src);
								this.RefreshPath(RepaintMode.Scene, true);
							}
						}
						else if (this.CheckTargetMove())
						{
							this.RefreshPath(RepaintMode.Scene, false);
						}
						if (this._changed && !this._isDragging)
						{
							this.FillWpIndexByDepth();
							this._changed = false;
						}
						int count = this._src.wps.Count;
						for (int i = 0; i < count; i++)
						{
							WpHandle wpHandle = this._wpsByDepth[i];
							bool flag2 = wpHandle.wpIndex == this._selectedWpIndex;
							Vector3 vector2 = this._src.wps[wpHandle.wpIndex];
							float num = (this._src.handlesDrawMode == HandlesDrawMode.Orthographic) ? (HandleUtility.GetHandleSize(vector2) * 0.2f) : this._src.perspectiveHandleSize;
							bool num2 = wpHandle.wpIndex >= 0 && wpHandle.wpIndex < (this._src.isClosedPath ? count : (count - 1));
							Vector3 vector3 = num2 ? ((wpHandle.wpIndex >= count - 1) ? this._src.transform.position : this._src.wps[wpHandle.wpIndex + 1]) : Vector3.zero;
							bool flag3 = num2 && Vector3.Distance(this._sceneCamTrans.position, vector2) < Vector3.Distance(this._sceneCamTrans.position, vector2 + Vector3.ClampMagnitude(vector3 - vector2, num * 1.75f));
							if (flag2)
							{
								Handles.color = Color.yellow;
							}
							else if (wpHandle.wpIndex == count - 1 && !this._src.isClosedPath)
							{
								Handles.color = this._wpColorEnd;
							}
							else
							{
								Handles.color = this._wpColor;
							}
							if (num2 & flag3)
							{
								this.DrawArrowFor(wpHandle.wpIndex, num, vector3);
							}
							int controlID = GUIUtility.GetControlID(FocusType.Passive);
							if (i == 0)
							{
								this._minHandleControlId = controlID;
							}
							vector2 = ((this._src.handlesType != 0) ? Handles.PositionHandle(vector2, Quaternion.identity) : Handles.FreeMoveHandle(vector2, Quaternion.identity, num, Vector3.one, Handles.SphereCap));
							this._src.wps[wpHandle.wpIndex] = vector2;
							int controlID2 = GUIUtility.GetControlID(FocusType.Passive);
							wpHandle.controlId = ((i == 0) ? (controlID2 - 1) : (controlID + 1));
							this._maxHandleControlId = controlID2;
							if (num2 && !flag3)
							{
								this.DrawArrowFor(wpHandle.wpIndex, num, vector3);
							}
							Vector3 position = this._sceneCamTrans.InverseTransformPoint(vector2) + new Vector3(num * 0.75f, 0.1f, 0f);
							position = this._sceneCamTrans.TransformPoint(position);
							if (this._src.showIndexes || this._src.showWpLength)
							{
								string text = (this._src.showIndexes && this._src.showWpLength) ? (wpHandle.wpIndex + 1 + "(" + this._src.path.wpLengths[wpHandle.wpIndex + 1].ToString("N2") + ")") : (this._src.showIndexes ? (wpHandle.wpIndex + 1).ToString() : this._src.path.wpLengths[wpHandle.wpIndex + 1].ToString("N2"));
								Handles.Label(position, text, flag2 ? EditorGUIUtils.handleSelectedLabelStyle : EditorGUIUtils.handlelabelStyle);
							}
						}
						Handles.color = this._src.pathColor;
						if (this._src.pathType == PathType.Linear)
						{
							Handles.DrawPolyLine(this._src.path.wps);
						}
						else if (this._src.path.nonLinearDrawWps != null)
						{
							Handles.DrawPolyLine(this._src.path.nonLinearDrawWps);
						}
						if (this._reselectAfterDrag && current.type == EventType.Repaint)
						{
							this._reselectAfterDrag = false;
						}
						if (!this._changed)
						{
							this._changed = this.Changed();
						}
						if (this._changed)
						{
							EditorUtility.SetDirty(this._src);
						}
					}
				}
			}
		}

		private void DORepaint(RepaintMode repaintMode, bool refreshWpIndexByDepth)
		{
			switch (repaintMode)
			{
			case RepaintMode.Scene:
				SceneView.RepaintAll();
				break;
			case RepaintMode.Inspector:
				EditorUtility.SetDirty(this._src);
				break;
			case RepaintMode.SceneAndInspector:
				EditorUtility.SetDirty(this._src);
				SceneView.RepaintAll();
				break;
			}
			if (refreshWpIndexByDepth)
			{
				this.FillWpIndexByDepth();
			}
		}

		private bool Changed()
		{
			if (GUI.changed)
			{
				return true;
			}
			if (this._lastSelectedWpIndex != this._selectedWpIndex)
			{
				this._lastSelectedWpIndex = this._selectedWpIndex;
				return true;
			}
			if (this.CheckTargetMove())
			{
				return true;
			}
			if (!(this._sceneCamTrans.position != this._lastSceneViewCamPosition) && !(this._sceneCamTrans.rotation != this._lastSceneViewCamRotation))
			{
				return false;
			}
			this._lastSceneViewCamPosition = this._sceneCamTrans.position;
			this._lastSceneViewCamRotation = this._sceneCamTrans.rotation;
			return true;
		}

		private void DrawArrowFor(int wpIndex, float handleSize, Vector3 arrowPointsAt)
		{
			Color color = Handles.color;
			Handles.color = this._arrowsColor;
			Vector3 vector = this._src.wps[wpIndex];
			Vector3 vector2 = arrowPointsAt - vector;
			if (vector2.magnitude >= handleSize * 1.75f)
			{
				Handles.ConeCap(wpIndex, vector + Vector3.ClampMagnitude(vector2, handleSize), Quaternion.LookRotation(vector2), handleSize * 0.65f);
			}
			Handles.color = color;
		}

		private void StoreSceneCamData()
		{
			if ((UnityEngine.Object)this._sceneCam == (UnityEngine.Object)null)
			{
				this._sceneCamStored = false;
			}
			else if (!this._sceneCamStored && !((UnityEngine.Object)this._sceneCam == (UnityEngine.Object)null))
			{
				this._sceneCamStored = true;
				this._lastSceneViewCamPosition = this._sceneCamTrans.position;
				this._lastSceneViewCamRotation = this._sceneCamTrans.rotation;
			}
		}

		private void FillWpIndexByDepth()
		{
			if (this._sceneCamStored)
			{
				int count = this._src.wps.Count;
				if (count != 0)
				{
					this._wpsByDepth.Clear();
					for (int i = 0; i < count; i++)
					{
						this._wpsByDepth.Add(new WpHandle(i));
					}
					this._wpsByDepth.Sort(delegate(WpHandle x, WpHandle y)
					{
						float num = Vector3.Distance(this._sceneCamTrans.position, this._src.wps[x.wpIndex]);
						float num2 = Vector3.Distance(this._sceneCamTrans.position, this._src.wps[y.wpIndex]);
						if (num > num2)
						{
							return -1;
						}
						if (num < num2)
						{
							return 1;
						}
						return 0;
					});
				}
			}
		}

		private void FindSelectedWaypointIndex()
		{
			this._lastSelectedWpIndex = this._selectedWpIndex;
			this._selectedWpIndex = -1;
			int count = this._src.wps.Count;
			if (count != 0)
			{
				int nearestControl = HandleUtility.nearestControl;
				if (nearestControl != 0 && nearestControl >= this._minHandleControlId && nearestControl <= this._maxHandleControlId)
				{
					int num = -1;
					for (int i = 0; i < count; i++)
					{
						int controlId = this._wpsByDepth[i].controlId;
						if (controlId != -1 && controlId != 0)
						{
							int wpIndex = this._wpsByDepth[i].wpIndex;
							if (controlId > nearestControl)
							{
								this._selectedWpIndex = this._wpsByDepth[(num == -1) ? i : num].wpIndex;
								this._lastCreatedWpIndex = -1;
								return;
							}
							if (controlId == nearestControl)
							{
								this._selectedWpIndex = wpIndex;
								this._lastCreatedWpIndex = -1;
								return;
							}
							num = i;
						}
					}
					if (this._selectedWpIndex == -1)
					{
						this._selectedWpIndex = this._wpsByDepth[num].wpIndex;
						this._lastCreatedWpIndex = -1;
					}
				}
			}
		}

		private void ResetPath(RepaintMode repaintMode)
		{
			this._src.wps.Clear();
			this._src.lastSrcPosition = this._src.transform.position;
			this._src.path = new Path(this._src.pathType, this._src.wps.ToArray(), 10, this._src.pathColor);
			this._wpsByDepth.Clear();
			this.ResetIndexes();
			this.DORepaint(repaintMode, false);
		}

		private void ResetIndexes()
		{
			this._selectedWpIndex = (this._lastSelectedWpIndex = (this._lastCreatedWpIndex = -1));
		}

		private bool CheckTargetMove()
		{
			if (this._src.lastSrcPosition != this._src.transform.position)
			{
				if (this._src.relative)
				{
					Vector3 b = this._src.transform.position - this._src.lastSrcPosition;
					int count = this._src.wps.Count;
					for (int i = 0; i < count; i++)
					{
						this._src.wps[i] = this._src.wps[i] + b;
					}
				}
				this._src.lastSrcPosition = this._src.transform.position;
				return true;
			}
			return false;
		}

		private void RefreshPath(RepaintMode repaintMode, bool refreshWpIndexByDepth)
		{
			if (this._src.wps.Count >= 1)
			{
				this._src.path.AssignDecoder(this._src.pathType);
				this._src.path.AssignWaypoints(this._src.GetFullWps(), false);
				this._src.path.FinalizePath(this._src.isClosedPath, AxisConstraint.None, this._src.transform.position);
				if (this._src.pathType != 0)
				{
					Path.RefreshNonLinearDrawWps(this._src.path);
				}
				this.DORepaint(repaintMode, refreshWpIndexByDepth);
			}
		}

		private void CopyWaypointsToClipboard()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Vector3[] waypoints = new[] { ");
			for (int i = 0; i < this._src.wps.Count; i++)
			{
				Vector3 vector = this._src.wps[i];
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(string.Format("new Vector3({0}f,{1}f,{2}f)", vector.x, vector.y, vector.z));
			}
			stringBuilder.Append(" };");
			EditorGUIUtility.systemCopyBuffer = stringBuilder.ToString();
		}
	}
}
