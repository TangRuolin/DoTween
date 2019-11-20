using DG.Tweening.Core.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DG.Tweening.Core
{
	internal static class TweenManager
	{
		internal enum CapacityIncreaseMode
		{
			TweenersAndSequences,
			TweenersOnly,
			SequencesOnly
		}

		private const int _DefaultMaxTweeners = 200;

		private const int _DefaultMaxSequences = 50;

		private const string _MaxTweensReached = "Max Tweens reached: capacity has automatically been increased from #0 to #1. Use DOTween.SetTweensCapacity to set it manually at startup";

		internal static int maxActive = 250;

		internal static int maxTweeners = 200;

		internal static int maxSequences = 50;

		internal static bool hasActiveTweens;

		internal static bool hasActiveDefaultTweens;

		internal static bool hasActiveLateTweens;

		internal static bool hasActiveFixedTweens;

		internal static int totActiveTweens;

		internal static int totActiveDefaultTweens;

		internal static int totActiveLateTweens;

		internal static int totActiveFixedTweens;

		internal static int totActiveTweeners;

		internal static int totActiveSequences;

		internal static int totPooledTweeners;

		internal static int totPooledSequences;

		internal static int totTweeners;

		internal static int totSequences;

		internal static bool isUpdateLoop;

		internal static Tween[] _activeTweens = new Tween[250];

		private static Tween[] _pooledTweeners = new Tween[200];

		private static readonly Stack<Tween> _PooledSequences = new Stack<Tween>();

		private static readonly List<Tween> _KillList = new List<Tween>(250);

		private static int _maxActiveLookupId = -1;

		private static bool _requiresActiveReorganization;

		private static int _reorganizeFromId = -1;

		private static int _minPooledTweenerId = -1;

		private static int _maxPooledTweenerId = -1;

		private static bool _despawnAllCalledFromUpdateLoopCallback;

		internal static TweenerCore<T1, T2, TPlugOptions> GetTweener<T1, T2, TPlugOptions>() where TPlugOptions : struct
		{
			if (TweenManager.totPooledTweeners > 0)
			{
				Type typeFromHandle = typeof(T1);
				Type typeFromHandle2 = typeof(T2);
				Type typeFromHandle3 = typeof(TPlugOptions);
				for (int num = TweenManager._maxPooledTweenerId; num > TweenManager._minPooledTweenerId - 1; num--)
				{
					Tween tween = TweenManager._pooledTweeners[num];
					if (tween != null && tween.typeofT1 == typeFromHandle && tween.typeofT2 == typeFromHandle2 && tween.typeofTPlugOptions == typeFromHandle3)
					{
						TweenerCore<T1, T2, TPlugOptions> obj = (TweenerCore<T1, T2, TPlugOptions>)tween;
						TweenManager.AddActiveTween(obj);
						TweenManager._pooledTweeners[num] = null;
						if (TweenManager._maxPooledTweenerId != TweenManager._minPooledTweenerId)
						{
							if (num == TweenManager._maxPooledTweenerId)
							{
								TweenManager._maxPooledTweenerId--;
							}
							else if (num == TweenManager._minPooledTweenerId)
							{
								TweenManager._minPooledTweenerId++;
							}
						}
						TweenManager.totPooledTweeners--;
						return obj;
					}
				}
				if (TweenManager.totTweeners >= TweenManager.maxTweeners)
				{
					TweenManager._pooledTweeners[TweenManager._maxPooledTweenerId] = null;
					TweenManager._maxPooledTweenerId--;
					TweenManager.totPooledTweeners--;
					TweenManager.totTweeners--;
				}
			}
			else if (TweenManager.totTweeners >= TweenManager.maxTweeners - 1)
			{
				int num2 = TweenManager.maxTweeners;
				int num3 = TweenManager.maxSequences;
				TweenManager.IncreaseCapacities(CapacityIncreaseMode.TweenersOnly);
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning("Max Tweens reached: capacity has automatically been increased from #0 to #1. Use DOTween.SetTweensCapacity to set it manually at startup".Replace("#0", num2 + "/" + num3).Replace("#1", TweenManager.maxTweeners + "/" + TweenManager.maxSequences));
				}
			}
			TweenerCore<T1, T2, TPlugOptions> tweenerCore = new TweenerCore<T1, T2, TPlugOptions>();
			TweenManager.totTweeners++;
			TweenManager.AddActiveTween(tweenerCore);
			return tweenerCore;
		}

		internal static Sequence GetSequence()
		{
			if (TweenManager.totPooledSequences > 0)
			{
				Sequence obj = (Sequence)TweenManager._PooledSequences.Pop();
				TweenManager.AddActiveTween(obj);
				TweenManager.totPooledSequences--;
				return obj;
			}
			if (TweenManager.totSequences >= TweenManager.maxSequences - 1)
			{
				int num = TweenManager.maxTweeners;
				int num2 = TweenManager.maxSequences;
				TweenManager.IncreaseCapacities(CapacityIncreaseMode.SequencesOnly);
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning("Max Tweens reached: capacity has automatically been increased from #0 to #1. Use DOTween.SetTweensCapacity to set it manually at startup".Replace("#0", num + "/" + num2).Replace("#1", TweenManager.maxTweeners + "/" + TweenManager.maxSequences));
				}
			}
			Sequence sequence = new Sequence();
			TweenManager.totSequences++;
			TweenManager.AddActiveTween(sequence);
			return sequence;
		}

		internal static void SetUpdateType(Tween t, UpdateType updateType, bool isIndependentUpdate)
		{
			if (!t.active || t.updateType == updateType)
			{
				t.updateType = updateType;
				t.isIndependentUpdate = isIndependentUpdate;
			}
			else
			{
				if (t.updateType == UpdateType.Normal)
				{
					TweenManager.totActiveDefaultTweens--;
					TweenManager.hasActiveDefaultTweens = (TweenManager.totActiveDefaultTweens > 0);
				}
				else if (t.updateType == UpdateType.Fixed)
				{
					TweenManager.totActiveFixedTweens--;
					TweenManager.hasActiveFixedTweens = (TweenManager.totActiveFixedTweens > 0);
				}
				else
				{
					TweenManager.totActiveLateTweens--;
					TweenManager.hasActiveLateTweens = (TweenManager.totActiveLateTweens > 0);
				}
				t.updateType = updateType;
				t.isIndependentUpdate = isIndependentUpdate;
				switch (updateType)
				{
				case UpdateType.Normal:
					TweenManager.totActiveDefaultTweens++;
					TweenManager.hasActiveDefaultTweens = true;
					break;
				case UpdateType.Fixed:
					TweenManager.totActiveFixedTweens++;
					TweenManager.hasActiveFixedTweens = true;
					break;
				default:
					TweenManager.totActiveLateTweens++;
					TweenManager.hasActiveLateTweens = true;
					break;
				}
			}
		}

		internal static void AddActiveTweenToSequence(Tween t)
		{
			TweenManager.RemoveActiveTween(t);
		}

		internal static int DespawnAll()
		{
			int result = TweenManager.totActiveTweens;
			for (int i = 0; i < TweenManager._maxActiveLookupId + 1; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (tween != null)
				{
					TweenManager.Despawn(tween, false);
				}
			}
			TweenManager.ClearTweenArray(TweenManager._activeTweens);
			TweenManager.hasActiveTweens = (TweenManager.hasActiveDefaultTweens = (TweenManager.hasActiveLateTweens = (TweenManager.hasActiveFixedTweens = false)));
			TweenManager.totActiveTweens = (TweenManager.totActiveDefaultTweens = (TweenManager.totActiveLateTweens = (TweenManager.totActiveFixedTweens = 0)));
			TweenManager.totActiveTweeners = (TweenManager.totActiveSequences = 0);
			TweenManager._maxActiveLookupId = (TweenManager._reorganizeFromId = -1);
			TweenManager._requiresActiveReorganization = false;
			if (TweenManager.isUpdateLoop)
			{
				TweenManager._despawnAllCalledFromUpdateLoopCallback = true;
			}
			return result;
		}

		internal static void Despawn(Tween t, bool modifyActiveLists = true)
		{
			if (t.onKill != null)
			{
				Tween.OnTweenCallback(t.onKill);
			}
			if (modifyActiveLists)
			{
				TweenManager.RemoveActiveTween(t);
			}
			if (t.isRecyclable)
			{
				switch (t.tweenType)
				{
				case TweenType.Sequence:
				{
					TweenManager._PooledSequences.Push(t);
					TweenManager.totPooledSequences++;
					Sequence sequence = (Sequence)t;
					int count = sequence.sequencedTweens.Count;
					for (int i = 0; i < count; i++)
					{
						TweenManager.Despawn(sequence.sequencedTweens[i], false);
					}
					break;
				}
				case TweenType.Tweener:
					if (TweenManager._maxPooledTweenerId == -1)
					{
						TweenManager._maxPooledTweenerId = TweenManager.maxTweeners - 1;
						TweenManager._minPooledTweenerId = TweenManager.maxTweeners - 1;
					}
					if (TweenManager._maxPooledTweenerId < TweenManager.maxTweeners - 1)
					{
						TweenManager._pooledTweeners[TweenManager._maxPooledTweenerId + 1] = t;
						TweenManager._maxPooledTweenerId++;
						if (TweenManager._minPooledTweenerId > TweenManager._maxPooledTweenerId)
						{
							TweenManager._minPooledTweenerId = TweenManager._maxPooledTweenerId;
						}
					}
					else
					{
						int num = TweenManager._maxPooledTweenerId;
						while (num > -1)
						{
							if (TweenManager._pooledTweeners[num] != null)
							{
								num--;
								continue;
							}
							TweenManager._pooledTweeners[num] = t;
							if (num < TweenManager._minPooledTweenerId)
							{
								TweenManager._minPooledTweenerId = num;
							}
							if (TweenManager._maxPooledTweenerId >= TweenManager._minPooledTweenerId)
							{
								break;
							}
							TweenManager._maxPooledTweenerId = TweenManager._minPooledTweenerId;
							break;
						}
					}
					TweenManager.totPooledTweeners++;
					break;
				}
			}
			else
			{
				switch (t.tweenType)
				{
				case TweenType.Sequence:
				{
					TweenManager.totSequences--;
					Sequence sequence2 = (Sequence)t;
					int count2 = sequence2.sequencedTweens.Count;
					for (int j = 0; j < count2; j++)
					{
						TweenManager.Despawn(sequence2.sequencedTweens[j], false);
					}
					break;
				}
				case TweenType.Tweener:
					TweenManager.totTweeners--;
					break;
				}
			}
			t.active = false;
			t.Reset();
		}

		internal static void PurgeAll()
		{
			for (int i = 0; i < TweenManager.totActiveTweens; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (tween != null)
				{
					tween.active = false;
					if (tween.onKill != null)
					{
						Tween.OnTweenCallback(tween.onKill);
					}
				}
			}
			TweenManager.ClearTweenArray(TweenManager._activeTweens);
			TweenManager.hasActiveTweens = (TweenManager.hasActiveDefaultTweens = (TweenManager.hasActiveLateTweens = (TweenManager.hasActiveFixedTweens = false)));
			TweenManager.totActiveTweens = (TweenManager.totActiveDefaultTweens = (TweenManager.totActiveLateTweens = (TweenManager.totActiveFixedTweens = 0)));
			TweenManager.totActiveTweeners = (TweenManager.totActiveSequences = 0);
			TweenManager._maxActiveLookupId = (TweenManager._reorganizeFromId = -1);
			TweenManager._requiresActiveReorganization = false;
			TweenManager.PurgePools();
			TweenManager.ResetCapacities();
			TweenManager.totTweeners = (TweenManager.totSequences = 0);
		}

		internal static void PurgePools()
		{
			TweenManager.totTweeners -= TweenManager.totPooledTweeners;
			TweenManager.totSequences -= TweenManager.totPooledSequences;
			TweenManager.ClearTweenArray(TweenManager._pooledTweeners);
			TweenManager._PooledSequences.Clear();
			TweenManager.totPooledTweeners = (TweenManager.totPooledSequences = 0);
			TweenManager._minPooledTweenerId = (TweenManager._maxPooledTweenerId = -1);
		}

		internal static void ResetCapacities()
		{
			TweenManager.SetCapacities(200, 50);
		}

		internal static void SetCapacities(int tweenersCapacity, int sequencesCapacity)
		{
			if (tweenersCapacity < sequencesCapacity)
			{
				tweenersCapacity = sequencesCapacity;
			}
			TweenManager.maxActive = tweenersCapacity + sequencesCapacity;
			TweenManager.maxTweeners = tweenersCapacity;
			TweenManager.maxSequences = sequencesCapacity;
			Array.Resize<Tween>(ref TweenManager._activeTweens, TweenManager.maxActive);
			Array.Resize<Tween>(ref TweenManager._pooledTweeners, tweenersCapacity);
			TweenManager._KillList.Capacity = TweenManager.maxActive;
		}

		internal static int Validate()
		{
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			int num = 0;
			for (int i = 0; i < TweenManager._maxActiveLookupId + 1; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (!tween.Validate())
				{
					num++;
					TweenManager.MarkForKilling(tween);
				}
			}
			if (num > 0)
			{
				TweenManager.DespawnTweens(TweenManager._KillList, false);
				for (int num2 = TweenManager._KillList.Count - 1; num2 > -1; num2--)
				{
					TweenManager.RemoveActiveTween(TweenManager._KillList[num2]);
				}
				TweenManager._KillList.Clear();
			}
			return num;
		}

		internal static void Update(UpdateType updateType, float deltaTime, float independentTime)
		{
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			TweenManager.isUpdateLoop = true;
			bool flag = false;
			int num = TweenManager._maxActiveLookupId + 1;
			for (int i = 0; i < num; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				float num2;
				if (tween != null && tween.updateType == updateType)
				{
					if (!tween.active)
					{
						flag = true;
						TweenManager.MarkForKilling(tween);
					}
					else if (tween.isPlaying)
					{
						tween.creationLocked = true;
						num2 = (tween.isIndependentUpdate ? independentTime : deltaTime) * tween.timeScale;
						if (!tween.delayComplete)
						{
							num2 = tween.UpdateDelay(tween.elapsedDelay + num2);
							if (num2 <= -1f)
							{
								flag = true;
								TweenManager.MarkForKilling(tween);
							}
							else if (!(num2 <= 0f))
							{
								if (tween.playedOnce && tween.onPlay != null)
								{
									Tween.OnTweenCallback(tween.onPlay);
								}
								goto IL_00d0;
							}
							continue;
						}
						goto IL_00d0;
					}
				}
				continue;
				IL_00d0:
				if (!tween.startupDone && !tween.Startup())
				{
					flag = true;
					TweenManager.MarkForKilling(tween);
				}
				else
				{
					float position = tween.position;
					bool flag2 = position >= tween.duration;
					int num3 = tween.completedLoops;
					if (tween.duration <= 0f)
					{
						position = 0f;
						num3 = ((tween.loops == -1) ? (tween.completedLoops + 1) : tween.loops);
					}
					else
					{
						if (tween.isBackwards)
						{
							position -= num2;
							while (position < 0f && num3 > 0)
							{
								position += tween.duration;
								num3--;
							}
						}
						else
						{
							position += num2;
							while (true)
							{
								if (!(position >= tween.duration))
								{
									break;
								}
								if (tween.loops != -1 && num3 >= tween.loops)
								{
									break;
								}
								position -= tween.duration;
								num3++;
							}
						}
						if (flag2)
						{
							num3--;
						}
						if (tween.loops != -1 && num3 >= tween.loops)
						{
							position = tween.duration;
						}
					}
					if (Tween.DoGoto(tween, position, num3, UpdateMode.Update))
					{
						flag = true;
						TweenManager.MarkForKilling(tween);
					}
				}
			}
			if (flag)
			{
				if (TweenManager._despawnAllCalledFromUpdateLoopCallback)
				{
					TweenManager._despawnAllCalledFromUpdateLoopCallback = false;
				}
				else
				{
					TweenManager.DespawnTweens(TweenManager._KillList, false);
					for (int num4 = TweenManager._KillList.Count - 1; num4 > -1; num4--)
					{
						TweenManager.RemoveActiveTween(TweenManager._KillList[num4]);
					}
				}
				TweenManager._KillList.Clear();
			}
			TweenManager.isUpdateLoop = false;
		}

		internal static int FilteredOperation(OperationType operationType, FilterType filterType, object id, bool optionalBool, float optionalFloat, object optionalObj = null, object[] optionalArray = null)
		{
			int num = 0;
			bool flag = false;
			int num2 = (optionalArray != null) ? optionalArray.Length : 0;
			for (int num3 = TweenManager._maxActiveLookupId; num3 > -1; num3--)
			{
				Tween tween = TweenManager._activeTweens[num3];
				if (tween != null && tween.active)
				{
					bool flag2 = false;
					switch (filterType)
					{
					case FilterType.All:
						flag2 = true;
						break;
					case FilterType.TargetOrId:
						flag2 = (id.Equals(tween.id) || id.Equals(tween.target));
						break;
					case FilterType.TargetAndId:
						flag2 = (id.Equals(tween.id) && optionalObj != null && optionalObj.Equals(tween.target));
						break;
					case FilterType.AllExceptTargetsOrIds:
					{
						flag2 = true;
						int num4 = 0;
						while (num4 < num2)
						{
							object obj = optionalArray[num4];
							if (!obj.Equals(tween.id) && !obj.Equals(tween.target))
							{
								num4++;
								continue;
							}
							flag2 = false;
							break;
						}
						break;
					}
					}
					if (flag2)
					{
						switch (operationType)
						{
						case OperationType.Despawn:
							num++;
							if (TweenManager.isUpdateLoop)
							{
								tween.active = false;
							}
							else
							{
								TweenManager.Despawn(tween, false);
								flag = true;
								TweenManager._KillList.Add(tween);
							}
							break;
						case OperationType.Complete:
						{
							bool autoKill = tween.autoKill;
							if (TweenManager.Complete(tween, false, (UpdateMode)((!(optionalFloat > 0f)) ? 1 : 0)))
							{
								num += ((!optionalBool) ? 1 : (autoKill ? 1 : 0));
								if (autoKill)
								{
									if (TweenManager.isUpdateLoop)
									{
										tween.active = false;
									}
									else
									{
										flag = true;
										TweenManager._KillList.Add(tween);
									}
								}
							}
							break;
						}
						case OperationType.Flip:
							if (TweenManager.Flip(tween))
							{
								num++;
							}
							break;
						case OperationType.Goto:
							TweenManager.Goto(tween, optionalFloat, optionalBool, UpdateMode.Goto);
							num++;
							break;
						case OperationType.Pause:
							if (TweenManager.Pause(tween))
							{
								num++;
							}
							break;
						case OperationType.Play:
							if (TweenManager.Play(tween))
							{
								num++;
							}
							break;
						case OperationType.PlayBackwards:
							if (TweenManager.PlayBackwards(tween))
							{
								num++;
							}
							break;
						case OperationType.PlayForward:
							if (TweenManager.PlayForward(tween))
							{
								num++;
							}
							break;
						case OperationType.Restart:
							if (TweenManager.Restart(tween, optionalBool))
							{
								num++;
							}
							break;
						case OperationType.Rewind:
							if (TweenManager.Rewind(tween, optionalBool))
							{
								num++;
							}
							break;
						case OperationType.SmoothRewind:
							if (TweenManager.SmoothRewind(tween))
							{
								num++;
							}
							break;
						case OperationType.TogglePause:
							if (TweenManager.TogglePause(tween))
							{
								num++;
							}
							break;
						case OperationType.IsTweening:
							if (tween.isComplete && tween.autoKill)
							{
								break;
							}
							num++;
							break;
						}
					}
				}
			}
			if (flag)
			{
				for (int num5 = TweenManager._KillList.Count - 1; num5 > -1; num5--)
				{
					TweenManager.RemoveActiveTween(TweenManager._KillList[num5]);
				}
				TweenManager._KillList.Clear();
			}
			return num;
		}

		internal static bool Complete(Tween t, bool modifyActiveLists = true, UpdateMode updateMode = UpdateMode.Goto)
		{
			if (t.loops == -1)
			{
				return false;
			}
			if (!t.isComplete)
			{
				Tween.DoGoto(t, t.duration, t.loops, updateMode);
				t.isPlaying = false;
				if (t.autoKill)
				{
					if (TweenManager.isUpdateLoop)
					{
						t.active = false;
					}
					else
					{
						TweenManager.Despawn(t, modifyActiveLists);
					}
				}
				return true;
			}
			return false;
		}

		internal static bool Flip(Tween t)
		{
			t.isBackwards = !t.isBackwards;
			return true;
		}

		internal static void ForceInit(Tween t, bool isSequenced = false)
		{
			if (!t.startupDone && !t.Startup() && !isSequenced)
			{
				if (TweenManager.isUpdateLoop)
				{
					t.active = false;
				}
				else
				{
					TweenManager.RemoveActiveTween(t);
				}
			}
		}

		internal static bool Goto(Tween t, float to, bool andPlay = false, UpdateMode updateMode = UpdateMode.Goto)
		{
			bool isPlaying = t.isPlaying;
			t.isPlaying = andPlay;
			t.delayComplete = true;
			t.elapsedDelay = t.delay;
			int num = Mathf.FloorToInt(to / t.duration);
			float num2 = to % t.duration;
			if (t.loops != -1 && num >= t.loops)
			{
				num = t.loops;
				num2 = t.duration;
			}
			else if (num2 >= t.duration)
			{
				num2 = 0f;
			}
			bool flag = Tween.DoGoto(t, num2, num, updateMode);
			if ((!andPlay & isPlaying) && !flag && t.onPause != null)
			{
				Tween.OnTweenCallback(t.onPause);
			}
			return flag;
		}

		internal static bool Pause(Tween t)
		{
			if (t.isPlaying)
			{
				t.isPlaying = false;
				if (t.onPause != null)
				{
					Tween.OnTweenCallback(t.onPause);
				}
				return true;
			}
			return false;
		}

		internal static bool Play(Tween t)
		{
			if (!t.isPlaying)
			{
				if (!t.isBackwards && !t.isComplete)
				{
					goto IL_0036;
				}
				if (t.isBackwards && (t.completedLoops > 0 || t.position > 0f))
				{
					goto IL_0036;
				}
			}
			return false;
			IL_0036:
			t.isPlaying = true;
			if (t.playedOnce && t.delayComplete && t.onPlay != null)
			{
				Tween.OnTweenCallback(t.onPlay);
			}
			return true;
		}

		internal static bool PlayBackwards(Tween t)
		{
			if (!t.isBackwards)
			{
				t.isBackwards = true;
				TweenManager.Play(t);
				return true;
			}
			return TweenManager.Play(t);
		}

		internal static bool PlayForward(Tween t)
		{
			if (t.isBackwards)
			{
				t.isBackwards = false;
				TweenManager.Play(t);
				return true;
			}
			return TweenManager.Play(t);
		}

		internal static bool Restart(Tween t, bool includeDelay = true)
		{
			bool num = !t.isPlaying;
			t.isBackwards = false;
			TweenManager.Rewind(t, includeDelay);
			t.isPlaying = true;
			if (num && t.playedOnce && t.delayComplete && t.onPlay != null)
			{
				Tween.OnTweenCallback(t.onPlay);
			}
			return true;
		}

		internal static bool Rewind(Tween t, bool includeDelay = true)
		{
			bool isPlaying = t.isPlaying;
			t.isPlaying = false;
			bool result = false;
			if (t.delay > 0f)
			{
				if (includeDelay)
				{
					result = (t.delay > 0f && t.elapsedDelay > 0f);
					t.elapsedDelay = 0f;
					t.delayComplete = false;
				}
				else
				{
					result = (t.elapsedDelay < t.delay);
					t.elapsedDelay = t.delay;
					t.delayComplete = true;
				}
			}
			if (t.position > 0f || t.completedLoops > 0 || !t.startupDone)
			{
				result = true;
				if ((!Tween.DoGoto(t, 0f, 0, UpdateMode.Goto) & isPlaying) && t.onPause != null)
				{
					Tween.OnTweenCallback(t.onPause);
				}
			}
			return result;
		}

		internal static bool SmoothRewind(Tween t)
		{
			bool result = false;
			if (t.delay > 0f)
			{
				result = (t.elapsedDelay < t.delay);
				t.elapsedDelay = t.delay;
				t.delayComplete = true;
			}
			if (t.position > 0f || t.completedLoops > 0 || !t.startupDone)
			{
				result = true;
				if (t.loopType == LoopType.Incremental)
				{
					t.PlayBackwards();
				}
				else
				{
					t.Goto(t.ElapsedDirectionalPercentage() * t.duration, false);
					t.PlayBackwards();
				}
			}
			else
			{
				t.isPlaying = false;
			}
			return result;
		}

		internal static bool TogglePause(Tween t)
		{
			if (t.isPlaying)
			{
				return TweenManager.Pause(t);
			}
			return TweenManager.Play(t);
		}

		internal static int TotalPooledTweens()
		{
			return TweenManager.totPooledTweeners + TweenManager.totPooledSequences;
		}

		internal static int TotalPlayingTweens()
		{
			if (!TweenManager.hasActiveTweens)
			{
				return 0;
			}
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			int num = 0;
			for (int i = 0; i < TweenManager._maxActiveLookupId + 1; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (tween != null && tween.isPlaying)
				{
					num++;
				}
			}
			return num;
		}

		internal static List<Tween> GetActiveTweens(bool playing)
		{
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			if (TweenManager.totActiveTweens <= 0)
			{
				return null;
			}
			int num = TweenManager.totActiveTweens;
			List<Tween> list = new List<Tween>(num);
			for (int i = 0; i < num; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (tween.isPlaying == playing)
				{
					list.Add(tween);
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
			return null;
		}

		internal static List<Tween> GetTweensById(object id, bool playingOnly)
		{
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			if (TweenManager.totActiveTweens <= 0)
			{
				return null;
			}
			int num = TweenManager.totActiveTweens;
			List<Tween> list = new List<Tween>(num);
			for (int i = 0; i < num; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (tween != null && object.Equals(id, tween.id) && (!playingOnly || tween.isPlaying))
				{
					list.Add(tween);
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
			return null;
		}

		internal static List<Tween> GetTweensByTarget(object target, bool playingOnly)
		{
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			if (TweenManager.totActiveTweens <= 0)
			{
				return null;
			}
			int num = TweenManager.totActiveTweens;
			List<Tween> list = new List<Tween>(num);
			for (int i = 0; i < num; i++)
			{
				Tween tween = TweenManager._activeTweens[i];
				if (tween.target == target && (!playingOnly || tween.isPlaying))
				{
					list.Add(tween);
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
			return null;
		}

		private static void MarkForKilling(Tween t)
		{
			t.active = false;
			TweenManager._KillList.Add(t);
		}

		private static void AddActiveTween(Tween t)
		{
			if (TweenManager._requiresActiveReorganization)
			{
				TweenManager.ReorganizeActiveTweens();
			}
			t.active = true;
			t.updateType = DOTween.defaultUpdateType;
			t.isIndependentUpdate = DOTween.defaultTimeScaleIndependent;
			t.activeId = (TweenManager._maxActiveLookupId = TweenManager.totActiveTweens);
			TweenManager._activeTweens[TweenManager.totActiveTweens] = t;
			if (t.updateType == UpdateType.Normal)
			{
				TweenManager.totActiveDefaultTweens++;
				TweenManager.hasActiveDefaultTweens = true;
			}
			else if (t.updateType == UpdateType.Fixed)
			{
				TweenManager.totActiveFixedTweens++;
				TweenManager.hasActiveFixedTweens = true;
			}
			else
			{
				TweenManager.totActiveLateTweens++;
				TweenManager.hasActiveLateTweens = true;
			}
			TweenManager.totActiveTweens++;
			if (t.tweenType == TweenType.Tweener)
			{
				TweenManager.totActiveTweeners++;
			}
			else
			{
				TweenManager.totActiveSequences++;
			}
			TweenManager.hasActiveTweens = true;
		}

		private static void ReorganizeActiveTweens()
		{
			if (TweenManager.totActiveTweens <= 0)
			{
				TweenManager._maxActiveLookupId = -1;
				TweenManager._requiresActiveReorganization = false;
				TweenManager._reorganizeFromId = -1;
			}
			else if (TweenManager._reorganizeFromId == TweenManager._maxActiveLookupId)
			{
				TweenManager._maxActiveLookupId--;
				TweenManager._requiresActiveReorganization = false;
				TweenManager._reorganizeFromId = -1;
			}
			else
			{
				int num = 1;
				int num2 = TweenManager._maxActiveLookupId + 1;
				TweenManager._maxActiveLookupId = TweenManager._reorganizeFromId - 1;
				for (int i = TweenManager._reorganizeFromId + 1; i < num2; i++)
				{
					Tween tween = TweenManager._activeTweens[i];
					if (tween == null)
					{
						num++;
					}
					else
					{
						tween.activeId = (TweenManager._maxActiveLookupId = i - num);
						TweenManager._activeTweens[i - num] = tween;
						TweenManager._activeTweens[i] = null;
					}
				}
				TweenManager._requiresActiveReorganization = false;
				TweenManager._reorganizeFromId = -1;
			}
		}

		private static void DespawnTweens(List<Tween> tweens, bool modifyActiveLists = true)
		{
			int count = tweens.Count;
			for (int i = 0; i < count; i++)
			{
				TweenManager.Despawn(tweens[i], modifyActiveLists);
			}
		}

		private static void RemoveActiveTween(Tween t)
		{
			int activeId = t.activeId;
			t.activeId = -1;
			TweenManager._requiresActiveReorganization = true;
			if (TweenManager._reorganizeFromId == -1 || TweenManager._reorganizeFromId > activeId)
			{
				TweenManager._reorganizeFromId = activeId;
			}
			TweenManager._activeTweens[activeId] = null;
			if (t.updateType == UpdateType.Normal)
			{
				if (TweenManager.totActiveDefaultTweens > 0)
				{
					TweenManager.totActiveDefaultTweens--;
					TweenManager.hasActiveDefaultTweens = (TweenManager.totActiveDefaultTweens > 0);
				}
				else
				{
					Debugger.LogRemoveActiveTweenError("totActiveDefaultTweens");
				}
			}
			else if (t.updateType == UpdateType.Fixed)
			{
				if (TweenManager.totActiveFixedTweens > 0)
				{
					TweenManager.totActiveFixedTweens--;
					TweenManager.hasActiveFixedTweens = (TweenManager.totActiveFixedTweens > 0);
				}
				else
				{
					Debugger.LogRemoveActiveTweenError("totActiveFixedTweens");
				}
			}
			else if (TweenManager.totActiveLateTweens > 0)
			{
				TweenManager.totActiveLateTweens--;
				TweenManager.hasActiveLateTweens = (TweenManager.totActiveLateTweens > 0);
			}
			else
			{
				Debugger.LogRemoveActiveTweenError("totActiveLateTweens");
			}
			TweenManager.totActiveTweens--;
			TweenManager.hasActiveTweens = (TweenManager.totActiveTweens > 0);
			if (t.tweenType == TweenType.Tweener)
			{
				TweenManager.totActiveTweeners--;
			}
			else
			{
				TweenManager.totActiveSequences--;
			}
			if (TweenManager.totActiveTweens < 0)
			{
				TweenManager.totActiveTweens = 0;
				Debugger.LogRemoveActiveTweenError("totActiveTweens");
			}
			if (TweenManager.totActiveTweeners < 0)
			{
				TweenManager.totActiveTweeners = 0;
				Debugger.LogRemoveActiveTweenError("totActiveTweeners");
			}
			if (TweenManager.totActiveSequences < 0)
			{
				TweenManager.totActiveSequences = 0;
				Debugger.LogRemoveActiveTweenError("totActiveSequences");
			}
		}

		private static void ClearTweenArray(Tween[] tweens)
		{
			int num = tweens.Length;
			for (int i = 0; i < num; i++)
			{
				tweens[i] = null;
			}
		}

		private static void IncreaseCapacities(CapacityIncreaseMode increaseMode)
		{
			int num = 0;
			int num2 = Mathf.Max((int)((float)TweenManager.maxTweeners * 1.5f), 200);
			int num3 = Mathf.Max((int)((float)TweenManager.maxSequences * 1.5f), 50);
			switch (increaseMode)
			{
			case CapacityIncreaseMode.TweenersOnly:
				num += num2;
				TweenManager.maxTweeners += num2;
				Array.Resize<Tween>(ref TweenManager._pooledTweeners, TweenManager.maxTweeners);
				break;
			case CapacityIncreaseMode.SequencesOnly:
				num += num3;
				TweenManager.maxSequences += num3;
				break;
			default:
				num += num2;
				TweenManager.maxTweeners += num2;
				TweenManager.maxSequences += num3;
				Array.Resize<Tween>(ref TweenManager._pooledTweeners, TweenManager.maxTweeners);
				break;
			}
			TweenManager.maxActive = TweenManager.maxTweeners + TweenManager.maxSequences;
			Array.Resize<Tween>(ref TweenManager._activeTweens, TweenManager.maxActive);
			if (num > 0)
			{
				TweenManager._KillList.Capacity += num;
			}
		}
	}
}
