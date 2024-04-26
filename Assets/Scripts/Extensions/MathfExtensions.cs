using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class MathfExtensions
    {
        public static bool Probability(float probability)
        {
            probability = Mathf.Clamp01(probability);

            if (probability == 0)
                return false;

            return Random.value <= probability;
        }

        public static void Probability(float probability, Action onTrue, Action onFalse = null)
        {
            if (Probability(probability))
                onTrue?.Invoke();
            else
                onFalse?.Invoke();
        }

        public static float Dispersion(this List<Vector3> vectors)
        {
            int count = vectors.Count;

            if (count == 0)
                return 0f;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                sum += vectors[i];
            }

            Vector3 mean = sum / count;

            float sumSquaredDistance = 0f;
            for (int i = 0; i < count; i++)
            {
                Vector3 deviation = vectors[i] - mean;
                sumSquaredDistance += deviation.sqrMagnitude;
            }

            float dispersion = sumSquaredDistance / count;
            return dispersion;
        }

        public static float Deviation(this List<Vector3> vectors)
        {
            int count = vectors.Count;

            if (count == 0)
                return 0;

            Vector3 sum = Vector3.zero;
            foreach (Vector3 vector in vectors)
            {
                sum += vector;
            }

            Vector3 mean = sum / count;

            float sumSquaredDeviation = 0f;
            foreach (Vector3 vector in vectors)
            {
                Vector3 deviation = vector - mean;
                sumSquaredDeviation += Vector3.Dot(deviation, deviation);
            }

            float rmsd = Mathf.Sqrt(sumSquaredDeviation / count);
            return rmsd;
        }
    }
}