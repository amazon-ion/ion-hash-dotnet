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

namespace IonHashDotnet
{
    using System;
    using IonDotnet;

    public class IonHashWriterBuilder
    {
        private IIonHasherProvider hasherProvider;
        private IIonWriter writer;

        // no public constructor
        private IonHashWriterBuilder()
        {
        }

        public static IonHashWriterBuilder Standard()
        {
            return new IonHashWriterBuilder();
        }

        public IonHashWriterBuilder WithWriter(IIonWriter writer)
        {
            this.writer = writer;
            return this;
        }

        public IonHashWriterBuilder WithHasherProvider(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        public IIonHashWriter Build()
        {
            if (this.hasherProvider == null || this.writer == null)
            {
                throw new ArgumentNullException("The Writer and HasherProvider must not be null");
            }

            return new IonHashWriter(this.writer, this.hasherProvider);
        }
    }
}
