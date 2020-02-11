/*
 * Copyright 2020 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

namespace Amazon.IonHashDotnet
{
    using System;
    using Amazon.IonDotnet;

    public class IonHashReaderBuilder
    {
        private IIonHasherProvider hasherProvider = null;
        private IIonReader reader = null;

        // no public constructor
        private IonHashReaderBuilder()
        {
        }

        public static IonHashReaderBuilder Standard()
        {
            return new IonHashReaderBuilder();
        }

        public IonHashReaderBuilder WithReader(IIonReader reader)
        {
            this.reader = reader;
            return this;
        }

        public IonHashReaderBuilder WithHasherProvider(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        public IIonHashReader Build()
        {
            if (this.hasherProvider == null || this.reader == null)
            {
                throw new ArgumentNullException("The Reader and HasherProvider must not be null");
            }

            return new IonHashReader(this.reader, this.hasherProvider);
        }
    }
}
