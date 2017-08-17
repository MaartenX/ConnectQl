# ExpressionExtensions Class
 

The expression extensions.


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;System.Linq.Expressions.ExpressionExtensions<br />
**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static class ExpressionExtensions
```

The ExpressionExtensions type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_CatchErrors">CatchErrors</a></td><td>
The catch errors.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_Eval">Eval</a></td><td>
Evaluates the expression.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_EvalExpression">EvalExpression</a></td><td>
Evaluates the expression and returns it as a constant expression.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_EvaluateAsValue">EvaluateAsValue</a></td><td>
Tries to evaluate the expression if it is a constant. If an error occurs, returns an <a href="T_ConnectQl_Results_Error">Error</a> object.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_Except">Except</a></td><td>
Returns the expression without the expressions to remove.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_FilterByAliases">FilterByAliases</a></td><td>
Leaves only filter parts that contain aliases specified in *sources*.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetFields">GetFields</a></td><td>
The get fields.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetFieldsFromSource">GetFieldsFromSource</a></td><td>
Gets the fields of the *source* that are used in the expression.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetGroupValues">GetGroupValues</a></td><td>
Gets the values for the specified field from a group.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetRowExpression__1">GetRowExpression(TParam)</a></td><td>
Creates a function that converts a row into a value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetRowFilter">GetRowFilter</a></td><td>
Creates a function that filters the rows based on a query.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_ReplaceParameter__1">ReplaceParameter(TExpression)</a></td><td>
Replaces the parameter in the specified expression.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_ReplaceParameters__1">ReplaceParameters(TExpression)</a></td><td>
Replaces the parameters in the specified expression.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_RewriteTasksToAsyncExpression">RewriteTasksToAsyncExpression(Expression)</a></td><td>
Rewrites <a href="T_ConnectQl_Expressions_TaskExpression">TaskExpression</a>s to an async expression (if needed).</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_RewriteTasksToAsyncExpression_1">RewriteTasksToAsyncExpression(LambdaExpression)</a></td><td>
Rewrites <a href="T_ConnectQl_Expressions_TaskExpression">TaskExpression</a>s to an async expression (if needed). a.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_Simplify">Simplify</a></td><td>
Evaluates all variables and function calls on constants.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SimplifyExpression">SimplifyExpression</a></td><td>
Evaluates all variables and function calls on constants.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SimplifyRanges">SimplifyRanges</a></td><td>
Evaluates all variables and function calls on constants.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SplitByAndExpressions">SplitByAndExpressions</a></td><td>
Splits an expression into multiple expression by And/AndAlsoexpressions.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SplitByOrExpressions">SplitByOrExpressions</a></td><td>
Splits an expression into multiple expression by Or/OrElse expressions. When or-expressions are nested inise , they are moved up the expression tree.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_ToRangedExpressionAsync">ToRangedExpressionAsync</a></td><td>
Replaces all fields in the *expressions* with the ranges for the fields found in *rows*. Leaves all fields of *ignoreAliases* intact.</td></tr></table>&nbsp;
<a href="#expressionextensions-class">Back to Top</a>

## See Also


#### Reference
<a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />