# Interpreter for Mini-PL programming language

I made this interpreter for the compilers course in the University of Helsinki. The objective was to build an interpreter for a given language.

The interpreter is written in C# as that was the suggested language then.

## Info

Source code can be found in the src folder along with a visual studio project file.

## Language grammar (in LL(1) form)

<prog> ::= <stmt-list>
<stmt-list> ::= <stmt> “;” <stmt-list-end>
<stmt-list-end> ::= <stmt-list>
| ε
<stmt> ::= “var” <var-ident> “:” <type> <optional-assignment>
| <var-ident> “:=” <expr>
| “for” <var-ident> “in” <expr> “..” <expr>
“do” <stmt-list> “end” “for”
| “read” <var-ident>
| “print” <expr>
| “assert” “(“ <expr> “)”
<optional-assignment> ::= “:=” <expr>
| ε
<expr> ::= <opnd> <expr-end>
| <unary-op> <opnd>
<expr-end> ::= <op> <opnd>
| ε
<opnd> ::= <int>
| <string>
| <var-ident>
| “(“ <expr> “)”
<type> ::= “int” | “string” | “bool”
<var-ident> ::= <ident>