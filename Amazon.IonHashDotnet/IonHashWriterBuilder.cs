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

    /// <summary>
    /// Build a new <see cref="IIonHashWriter"/> for the given <see cref="IIonWriter"/>
    /// and <see cref="IIonHasherProvider"/>.
    /// <p/>
    /// Instances of this class are not thread-safe.
    /// </summary>
    public class IonHashWriterBuilder
    {
        private IIonHasherProvider hasherProvider;
        private IIonWriter writer;

        // no public constructor
        private IonHashWriterBuilder()
        {
        }

        /// <summary>
        /// The standard builder of <see cref="IonHashWriterBuilder"/>s.
        /// </summary>
        /// <returns>The standard builder.</returns>
        public static IonHashWriterBuilder Standard()
        {
            return new IonHashWriterBuilder();
        }

        /// <summary>
        /// Specifies the writer to compute hashes over.
        /// </summary>
        /// <param name="writer">The IIonWriter to compute hashes over.</param>
        /// <returns>This builder.</returns>
        public IonHashWriterBuilder WithWriter(IIonWriter writer)
        {
            this.writer = writer;
            return this;
        }

        /// <summary>
        /// Specifies the hasher provider to use.
        /// </summary>
        /// <param name="hasherProvider">The IIonHasherProvider instance to use.</param>
        /// <returns>This builder.</returns>
        public IonHashWriterBuilder WithHasherProvider(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        /// <summary>
        /// Constructs a new IIonHashWriter, which decorates the IIonWriter with hashes.
        /// </summary>
        /// <returns>A new IIonHashWriter object.</returns>
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
