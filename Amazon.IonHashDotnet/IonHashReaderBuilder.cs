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
    /// Build a new <see cref="IIonHashReader"/> for the given <see cref="IIonReader"/>
    /// and <see cref="IIonHasherProvider"/>.
    /// <p/>
    /// Instances of this class are not thread-safe.
    /// </summary>
    public class IonHashReaderBuilder
    {
        private IIonHasherProvider hasherProvider = null;
        private IIonReader reader = null;

        // no public constructor
        private IonHashReaderBuilder()
        {
        }

        /// <summary>
        /// The standard builder of <see cref="IonHashReaderBuilder"/>s.
        /// </summary>
        /// <returns>The standard builder.</returns>
        public static IonHashReaderBuilder Standard()
        {
            return new IonHashReaderBuilder();
        }

        /// <summary>
        /// Specifies the reader to compute hashes over.
        /// </summary>
        /// <param name="reader">The IIonReader to compute hasher over.</param>
        /// <returns>This builder.</returns>
        public IonHashReaderBuilder WithReader(IIonReader reader)
        {
            this.reader = reader;
            return this;
        }

        /// <summary>
        /// Specifies the hasher provider to use.
        /// </summary>
        /// <param name="hasherProvider">The IIonHasherProvider instance to use.</param>
        /// <returns>This builder.</returns>
        public IonHashReaderBuilder WithHasherProvider(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        /// <summary>
        /// Constructs a new IIonHashReader, which decorates the IIonReader with hashes.
        /// </summary>
        /// <returns>A new IIonHashReader object.</returns>
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
