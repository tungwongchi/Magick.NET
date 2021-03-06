﻿// Copyright 2013-2020 Dirk Lemstra <https://github.com/dlemstra/Magick.NET/>
//
// Licensed under the ImageMagick License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at
//
//   https://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.

using ImageMagick;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if Q8
using QuantumType = System.Byte;
#elif Q16
using QuantumType = System.UInt16;
#elif Q16HDRI
using QuantumType = System.Single;
#else
#error Not implemented!
#endif

namespace Magick.NET
{
    [ExcludeFromCodeCoverage]
    internal static class ColorAssert
    {
        public static void AreEqual(IMagickColor<QuantumType> expected, IMagickColor<QuantumType> actual)
        {
            Assert.IsNotNull(actual);

#if Q16HDRI
            /* Allow difference of 1 due to rounding issues */
            QuantumType delta = 1;
#else
            QuantumType delta = 0;
#endif

            AreEqual(expected.R, actual.R, expected, actual, delta, "R");
            AreEqual(expected.G, actual.G, expected, actual, delta, "G");
            AreEqual(expected.B, actual.B, expected, actual, delta, "B");
            AreEqual(expected.A, actual.A, expected, actual, delta, "A");
        }

        public static void AreEqual(IMagickColor<QuantumType> expected, IMagickImage<QuantumType> image, int x, int y)
        {
            using (var pixels = image.GetPixelsUnsafe())
            {
                AreEqual(expected, pixels.GetPixel(x, y));
            }
        }

        public static void AreNotEqual(IMagickColor<QuantumType> notExpected, IMagickColor<QuantumType> actual)
        {
            if (notExpected.R == actual.R && notExpected.G == actual.G &&
               notExpected.B == actual.B && notExpected.A == actual.A)
                Assert.Fail("Colors are the same (" + actual.ToString() + ")");
        }

        public static void AreNotEqual(IMagickColor<QuantumType> notExpected, IMagickImage<QuantumType> image, int x, int y)
        {
            using (var collection = image.GetPixelsUnsafe())
            {
                AreNotEqual(notExpected, collection.GetPixel(x, y));
            }
        }

        public static void IsTransparent(float alpha)
            => Assert.AreEqual(0, alpha);

        public static void IsNotTransparent(float alpha)
            => Assert.AreEqual(Quantum.Max, alpha);

        private static void AreEqual(IMagickColor<QuantumType> expected, IPixel<QuantumType> actual)
            => AreEqual(expected, actual.ToColor());

        private static void AreEqual(QuantumType expected, QuantumType actual, IMagickColor<QuantumType> expectedColor, IMagickColor<QuantumType> actualColor, float delta, string channel)
        {
#if Q16HDRI
            if (double.IsNaN(actual))
                actual = 0;
#endif

            Assert.AreEqual(expected, actual, delta, channel + " is not equal (" + expectedColor.ToString() + " != " + actualColor.ToString() + ")");
        }

        private static void AreNotEqual(IMagickColor<QuantumType> expected, IPixel<QuantumType> actual)
            => AreNotEqual(expected, actual.ToColor());
    }
}
