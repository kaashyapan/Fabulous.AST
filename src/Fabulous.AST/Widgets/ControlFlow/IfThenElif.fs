namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fantomas.FCS.Text
open Fabulous.AST.StackAllocatedCollections
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak

module IfThenElif =
    let ElifExpr = Attributes.defineWidget "ElifExpr"

    let ElseExpr = Attributes.defineWidget "ElseExpr"

    let WidgetKey =
        Widgets.register "IfThen" (fun widget ->

            let elifExpr = Helpers.tryGetNodeFromWidget<ExprIfThenNode list> widget ElifExpr

            let ifTenElifExpr =
                match elifExpr with
                | ValueSome ifTenElifExpr -> ifTenElifExpr
                | ValueNone -> []

            let elseExpr = Helpers.tryGetNodeFromWidget<Expr> widget ElseExpr

            let elseExpr =
                match elseExpr with
                | ValueSome elseExpr -> Some(SingleTextNode("else", Range.Zero), elseExpr)
                | ValueNone -> None

            ExprIfThenElifNode(
                [ for elifExpr in ifTenElifExpr do
                      elifExpr ],
                elseExpr,
                Range.Zero
            ))

[<AutoOpen>]
module IfThenElifBuilders =
    type Fabulous.AST.Ast with

        static member inline ElIfElse(elseExpr: WidgetBuilder<Expr>) =
            SingleChildBuilder<ExprIfThenElifNode, ExprIfThenNode list>(
                IfThenElif.WidgetKey,
                IfThenElif.ElifExpr,
                AttributesBundle(
                    StackList.empty(),
                    ValueSome [| IfThenElif.ElseExpr.WithValue(elseExpr.Compile()) |],
                    ValueNone
                )
            )

[<Extension>]
type IfThenIfYieldExtensions =
    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<'parent, ModuleDecl>,
            x: WidgetBuilder<ExprIfThenElifNode>
        ) : CollectionContent =
        let node = Tree.compile x
        let expIfThen = Expr.IfThenElif(node)
        let moduleDecl = ModuleDecl.DeclExpr expIfThen
        let widget = Ast.EscapeHatch(moduleDecl).Compile()
        { Widgets = MutStackArray1.One(widget) }
