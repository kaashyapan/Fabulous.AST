namespace Fabulous.AST.Tests.TypeDefinitions

open Fabulous.AST.Tests
open Fantomas.FCS.Text
open Fantomas.Core.SyntaxOak
open NUnit.Framework

open Fabulous.AST

open type Fabulous.AST.Ast

module Union =

    [<Test>]
    let ``Produces an union`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }

        |> produces
            """

type Colors =
    | Red
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union with fields`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionParameterizedCase("Red") {
                    Field("a", Type.FromString "string")
                    Field("b", Type.FromString "int")
                }

                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }

        |> produces
            """

type Colors =
    | Red of a: string * b: int
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union with SingleTextNode`` () =
        AnonymousModule() {
            Union(SingleTextNode("Colors", Range.Zero)) {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }
        |> produces
            """

type Colors =
    | Red
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union using Widget and escape hatch`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
                EscapeHatch(UnionCaseNode(None, None, None, SingleTextNode("Black", Range.Zero), [], Range.Zero))

            }
        }
        |> produces
            """

type Colors =
    | Red
    | Green
    | Blue
    | Yellow
    | Black

"""

    [<Test>]
    let ``Produces an union with static member`` () =
        let expr = Expr.Constant(Constant.FromText(SingleTextNode("\"\"", Range.Zero)))
        let memberNode = (Method("A") { EscapeHatch(expr) }).isStatic()

        AnonymousModule() { (Union("Colors") { UnionCase("Red") }).members() { memberNode } }
        |> produces
            """

type Colors =
    | Red

    static member A = ""

"""

    [<Test>]
    let ``Produces an union with member`` () =
        let memberNode =
            MemberDefn.Member(
                BindingNode(
                    None,
                    None,
                    MultipleTextsNode([ SingleTextNode("member", Range.Zero) ], Range.Zero),
                    false,
                    None,
                    None,
                    Choice1Of2(
                        IdentListNode(
                            [ IdentifierOrDot.Ident(SingleTextNode("this", Range.Zero))
                              IdentifierOrDot.Ident(SingleTextNode(".", Range.Zero))
                              IdentifierOrDot.Ident(SingleTextNode("A", Range.Zero)) ],
                            Range.Zero
                        )
                    ),
                    None,
                    List.Empty,
                    None,
                    SingleTextNode("=", Range.Zero),
                    Expr.Constant(Constant.FromText(SingleTextNode("\"\"", Range.Zero))),
                    Range.Zero
                )
            )

        AnonymousModule() { (Union("Colors") { UnionCase("Red") }).members() { EscapeHatch(memberNode) } }
        |> produces
            """

type Colors =
    | Red

    member this.A = ""

"""

    [<Test>]
    let ``Produces an union with attribute`` () =
        AnonymousModule() { (Union("Colors") { UnionCase("Red") }).attributes([ "Test" ]) }
        |> produces
            """

[<Test>]
type Colors = | Red
"""

    [<Test>]
    let ``Produces an union case with attributes`` () =
        AnonymousModule() {
            (Union("Colors") { UnionCase("Red").attributes([ "Obsolete"; "Test" ]) })
                .attributes([ "Test" ])
        }
        |> produces
            """

[<Test>]
type Colors = | [<Obsolete; Test>] Red
"""

    [<Test>]
    let ``Produces an union with TypeParams`` () =
        AnonymousModule() {
            Union("Colors", [ "'other" ]) {
                UnionParameterizedCase("Red") {
                    Field("a", Type.FromString "string")
                    Field("b", Type.FromString "'other")
                }

                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }

        |> produces
            """

type Colors<'other> =
    | Red of a: string * b: 'other
    | Green
    | Blue
    | Yellow

"""
