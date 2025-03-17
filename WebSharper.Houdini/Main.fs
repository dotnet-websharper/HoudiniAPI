namespace WebSharper.Houdini

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let NumberType t = (T<float> + t)

    let CSSStyleValue =
        Class "CSSStyleValue"
        |+> Static [
            "parse" => T<string>?property * T<string>?value ^-> TSelf
            "parseAll" => T<string>?property * T<string>?value ^-> !| TSelf
        ]

    let CSSImageValue =
        Class "CSSImageValue"
        |=> Inherits CSSStyleValue

    let CSSKeywordValue =
        Class "CSSKeywordValue"
        |=> Inherits CSSStyleValue
        |+> Static [
            Constructor (T<string>?value)
        ]
        |+> Instance [
            "value" =@ T<string>
        ]

    let CSSNumericValue =
        Class "CSSNumericValue"

    let NumericValueType = NumberType CSSNumericValue.Type

    let CSSMathValue =
        Class "CSSMathValue"
        |=> Inherits CSSNumericValue
        |+> Instance [
            "operator" =? T<string>
        ]

    let MathValueType = !| NumericValueType

    let CSSMathSum =
        Class "CSSMathSum"
        |=> Inherits CSSMathValue
        |+> Static [
            Constructor MathValueType?values
        ]
        |+> Instance [
            "values" =? MathValueType
        ]

    let CSSMathProduct =
        Class "CSSMathProduct"
        |=> Inherits CSSMathValue
        |+> Static [
            Constructor MathValueType?args
        ]
        |+> Instance [
            "values" =? MathValueType
        ]

    let CSSUnitValue =
        Class "CSSUnitValue"
        |=> Inherits CSSNumericValue
        |+> Static [
            Constructor (T<float>?value * T<string>?unit)
        ]
        |+> Instance [
            "value" =@ T<float>
            "unit" =? T<string>
        ]

    CSSNumericValue
    |=> Inherits CSSStyleValue
    |+> Static [
        "parse" => T<string>?cssText ^-> TSelf
    ]
    |+> Instance [
        "add" => NumericValueType?number ^-> CSSMathSum
        "sub" => NumericValueType?number ^-> CSSMathSum
        "mul" => NumericValueType?number ^-> CSSMathProduct
        "div" => NumericValueType?number ^-> CSSMathProduct
        "min" => (!| NumericValueType)?numbers ^-> CSSUnitValue
        "max" => (!| NumericValueType)?numbers ^-> CSSUnitValue
        "equals" => NumericValueType?number ^-> T<bool>
        "to" => T<string>?unit ^-> CSSMathSum
        "toSum" => (!| T<string>)?units ^-> CSSMathSum
        "type" => T<unit> ^-> T<obj>
    ]    
    |> ignore

    let CSSTransformComponent =
        Class "CSSTransformComponent"
        |+> Instance [
            "is2D" =? T<bool>
            
            "toMatrix" => T<unit> ^-> T<DOMMatrix>
            "toMatrix" => T<unit> ^-> T<string>
        ]

    let CSSUnparsedValue =
        Class "CSSUnparsedValue"

    let CSSUnparsedValueForEachFn = T<obj>?currentValue * !?T<int>?index * !?CSSUnparsedValue?index ^-> T<unit>
    let CSSUnparsedValueConstructorType = !| (T<string> + T<obj>)

    CSSUnparsedValue       
    |=> Inherits CSSStyleValue
    |+> Static [
        Constructor CSSUnparsedValueConstructorType?members
    ]
    |+> Instance [
        "length" =? T<int> // Read-only property

        "entries" => T<obj>?obj ^-> T<obj>
        "forEach" => CSSUnparsedValueForEachFn?callbackFn * !?T<obj>?thisArg ^-> T<unit>
        "keys" => T<unit> ^-> T<Array>
        "values" => T<unit> ^-> T<Array>
    ]
    |> ignore

    let CSSTransformValue =

        let forEachFn = T<obj>?currentValue * !?T<int>?index * !?TSelf?index ^-> T<unit>

        Class "CSSTransformValue"
        |=> Inherits CSSStyleValue
        |+> Static [
            Constructor (!| CSSTransformComponent)?transforms
        ]
        |+> Instance [
            "length" =? T<int> // Read-only property
            "is2D" =? T<bool> // Read-only property

            "toMatrix" => T<unit> ^-> T<DOMMatrix>
            "entries" => T<obj>?obj ^-> T<obj>
            "forEach" => forEachFn?callbackFn * !?T<obj>?thisArg ^-> T<unit>
            "keys" => T<unit> ^-> T<Array>
            "values" => T<unit> ^-> T<Array>
        ]

    let PaintWorkletGlobalScope =
        Class "PaintWorkletGlobalScope"
        |+> Instance [
            "devicePixelRatio" =? T<float>
            
            "registerPaint" => T<string>?name * T<obj>?classRef ^-> T<unit>
        ]

    let StylePropertyMapReadOnly =

        let forEachFn = T<obj>?currentValue * !?T<int>?index * !?TSelf?index ^-> T<unit>

        Class "StylePropertyMapReadOnly"
        |+> Instance [
            "size" =? T<int> // Read-only unsigned long integer

            "entries" => T<unit> ^-> T<obj>
            "forEach" => (forEachFn?callbackFn * !?T<obj>?thisArg) ^-> T<unit> // Callback with optional thisArg
            "get" => T<string>?property ^-> CSSStyleValue // Returns a CSSStyleValue or null
            "getAll" => T<string>?property ^-> !| CSSStyleValue // Returns an array of CSSStyleValue
            "has" => T<string>?property ^-> T<bool> // Checks if the property exists
            "keys" => T<unit> ^-> T<Array> // Returns an array iterator of keys
            "values" => T<unit> ^-> T<Array> // Returns an array iterator of values
        ]

    let AddModuleOptions = 
        Pattern.Config "AddModuleOptions" {
            Required = []
            Optional = [
                "credentials", T<string>
            ]
        }

    let Worklet = 
        Class "Worklet"
        |+> Instance [
            "addModule" => T<string>?moduleURL * !?AddModuleOptions?options ^-> T<Promise<_>>[T<unit>]
        ]

    let StylePropertyMap =
        Class "StylePropertyMap"
        |=> Inherits StylePropertyMapReadOnly
        |+> Instance [            
            "append" => T<string>?property * T<obj>?value ^-> T<unit>
            "delete" => T<string>?property ^-> T<unit>
            "set" => T<string>?property * T<obj>?value ^-> T<unit>
            "clear" => T<unit> ^-> T<unit>
        ]

    let CSSPropertyDefinition =
        Pattern.Config "CSSPropertyDefinition" {
            Required = [ "name", T<string> ]
            Optional = [
                "syntax", T<string>
                "inherits", T<bool>
                "initialValue", T<string>
            ]
        }

    let CSS =
        Class "CSS"
        |+> Static [
            "registerProperty" => CSSPropertyDefinition?propertyDefinition ^-> T<unit>
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Houdini" [
                CSS
                CSSPropertyDefinition
                StylePropertyMap
                Worklet
                AddModuleOptions
                StylePropertyMapReadOnly
                PaintWorkletGlobalScope
                CSSTransformValue
                CSSUnparsedValue
                CSSTransformComponent
                CSSUnitValue
                CSSMathProduct
                CSSMathSum
                CSSMathValue
                CSSNumericValue
                CSSKeywordValue
                CSSImageValue
                CSSStyleValue
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
