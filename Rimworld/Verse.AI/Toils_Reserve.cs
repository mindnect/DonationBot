using System;
using System.Collections.Generic;

namespace Verse.AI
{
	public static class Toils_Reserve
	{
		public static Toil Reserve(TargetIndex ind, int maxPawns = 1)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				if (!toil.actor.Reserve(toil.actor.jobs.curJob.GetTarget(ind), maxPawns))
				{
					toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable, true);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			toil.atomicWithPrevious = true;
			return toil;
		}

		public static Toil ReserveQueue(TargetIndex ind, int maxPawns = 1)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				List<LocalTargetInfo> targetQueue = toil.actor.jobs.curJob.GetTargetQueue(ind);
				if (targetQueue != null)
				{
					for (int i = 0; i < targetQueue.Count; i++)
					{
						if (!toil.actor.Reserve(targetQueue[i], maxPawns))
						{
							toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable, true);
						}
					}
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			toil.atomicWithPrevious = true;
			return toil;
		}

		public static Toil Release(TargetIndex ind)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				toil.actor.Map.reservationManager.Release(toil.actor.jobs.curJob.GetTarget(ind), toil.actor);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			toil.atomicWithPrevious = true;
			return toil;
		}
	}
}
