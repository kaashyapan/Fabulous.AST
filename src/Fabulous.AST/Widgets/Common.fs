namespace Fabulous.AST

open System
open FSharp.Compiler.Text
open Fantomas.Core.SyntaxOak

type AccessControl =
    | Public
    | Private
    | Internal

[<AutoOpen>]
module Auxiliary =
    type Type with

        /// <summary>Create a type from a string.</summary>
        /// <exception cref="System.InvalidOperationException">
        /// Your input should be a single string text identifier.
        /// Nothing more complex.
        /// </exception>
        static member FromString(typeName: string) : Type =
            // TODO: consider validating the input here.
            // If something complex was passed in, it would be nice to throw an exception.
            // For now, we just assume that the input is valid.
            // Bad example would be: "int -> int", use Type.Funs instead.
            IdentListNode([ IdentifierOrDot.Ident(SingleTextNode(typeName, Range.Zero)) ], Range.Zero)
            |> Type.LongIdent
