namespace Fabulous.AST.Tests.ControlFlow

open Fantomas.FCS.Text
open Fabulous.AST
open Fabulous.AST.Tests

open type Ast
open Fantomas.Core.SyntaxOak
open NUnit.Framework

module IfThen =
    [<Test>]
    let ``Produces if-then expression with EscapeHatch`` () =
        let ifExp =
            Expr.InfixApp(
                ExprInfixAppNode(
                    Expr.Ident(SingleTextNode("x", Range.Zero)),
                    SingleTextNode("=", Range.Zero),
                    Expr.Ident(SingleTextNode("12", Range.Zero)),
                    Range.Zero
                )
            )

        let thenExpr =
            Expr.Constant(
                Constant.Unit(UnitNode(SingleTextNode("(", Range.Zero), SingleTextNode(")", Range.Zero), Range.Zero))
            )

        let thenExpr =
            Expr.CompExprBody(
                ExprCompExprBodyNode([ ComputationExpressionStatement.OtherStatement(thenExpr) ], Range.Zero)
            )

        AnonymousModule() { IfThen(EscapeHatch(ifExp)) { EscapeHatch(thenExpr) } }
        |> produces
            """

if x = 12 then
    ()
"""
