﻿using PMFightAcademy.Client.Models;
using System.ComponentModel.DataAnnotations;

namespace PMFightAcademy.Client.Contract
{
    /// <summary>
    /// Contract for getting data with paggination.
    /// </summary>
    /// <typeparam name="T">The type of result data.</typeparam>
    public class GetDataContract<T> where T: class
    {
        /// <summary>
        /// The array of <typeparamref name="T"/> elements.
        /// </summary>
        [Required]
        public T[] Data { get; set; }

        /// <summary>
        /// Paggination state.
        /// </summary>
        [Required]
        public Paggination Paggination { get; set; }
    }
}