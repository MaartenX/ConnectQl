# ExecutionContextExpression Methods
 

The <a href="T_ConnectQl_Expressions_ExecutionContextExpression">ExecutionContextExpression</a> type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/ee378284" target="_blank">Accept</a></td><td>
Dispatches to the specific visit method for this node type. For example, <a href="http://msdn2.microsoft.com/en-us/library/bb357368" target="_blank">MethodCallExpression</a> calls the <a href="http://msdn2.microsoft.com/en-us/library/dd323893" target="_blank">VisitMethodCall(MethodCallExpression)</a>.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/bsc2ak47" target="_blank">Equals</a></td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/4k87zsw7" target="_blank">Finalize</a></td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/zdee4b3y" target="_blank">GetHashCode</a></td><td>
Serves as the default hash function.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dfwy45w9" target="_blank">GetType</a></td><td>
Gets the <a href="http://msdn2.microsoft.com/en-us/library/42892f65" target="_blank">Type</a> of the current instance.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/57ctke0a" target="_blank">MemberwiseClone</a></td><td>
Creates a shallow copy of the current <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dd294075" target="_blank">Reduce</a></td><td>
Reduces this node to a simpler expression. If CanReduce returns true, this should return a valid expression. This method can return another node which itself must be reduced.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dd267939" target="_blank">ReduceAndCheck</a></td><td>
Reduces this node to a simpler expression. If CanReduce returns true, this should return a valid expression. This method can return another node which itself must be reduced.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dd324006" target="_blank">ReduceExtensions</a></td><td>
Reduces the expression to a known node type (that is not an Extension node) or just returns the expression if it is already a known type.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Expressions_ExecutionContextExpression_ToString">ToString</a></td><td>
The to string.
 (Overrides <a href="http://msdn2.microsoft.com/en-us/library/bb354391" target="_blank">Expression.ToString()</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_ConnectQl_Expressions_ExecutionContextExpression_VisitChildren">VisitChildren</a></td><td>
The visit children.
 (Overrides <a href="http://msdn2.microsoft.com/en-us/library/dd324123" target="_blank">Expression.VisitChildren(ExpressionVisitor)</a>.)</td></tr></table>&nbsp;
<a href="#executioncontextexpression-methods">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_CatchErrors">CatchErrors</a></td><td>
The catch errors.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_Eval">Eval</a></td><td>
Evaluates the expression.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_EvalExpression">EvalExpression</a></td><td>
Evaluates the expression and returns it as a constant expression.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_EvaluateAsValue">EvaluateAsValue</a></td><td>
Tries to evaluate the expression if it is a constant. If an error occurs, returns an <a href="T_ConnectQl_Results_Error">Error</a> object.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_Except">Except</a></td><td>
Returns the expression without the expressions to remove.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_FilterByAliases">FilterByAliases</a></td><td>
Leaves only filter parts that contain aliases specified in *sources*.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetFields">GetFields</a></td><td>
The get fields.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetFieldsFromSource">GetFieldsFromSource</a></td><td>
Gets the fields of the *source* that are used in the expression.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetRowExpression__1">GetRowExpression(TParam)</a></td><td>
Creates a function that converts a row into a value.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_GetRowFilter">GetRowFilter</a></td><td>
Creates a function that filters the rows based on a query.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_RewriteTasksToAsyncExpression">RewriteTasksToAsyncExpression</a></td><td>
Rewrites <a href="T_ConnectQl_Expressions_TaskExpression">TaskExpression</a>s to an async expression (if needed).
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_Simplify">Simplify</a></td><td>
Evaluates all variables and function calls on constants.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SimplifyExpression">SimplifyExpression</a></td><td>
Evaluates all variables and function calls on constants.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SimplifyRanges">SimplifyRanges</a></td><td>
Evaluates all variables and function calls on constants.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SplitByAndExpressions">SplitByAndExpressions</a></td><td>
Splits an expression into multiple expression by And/AndAlsoexpressions.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_System_Linq_Expressions_ExpressionExtensions_SplitByOrExpressions">SplitByOrExpressions</a></td><td>
Splits an expression into multiple expression by Or/OrElse expressions. When or-expressions are nested inise , they are moved up the expression tree.
 (Defined by <a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions</a>.)</td></tr></table>&nbsp;
<a href="#executioncontextexpression-methods">Back to Top</a>

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_ExecutionContextExpression">ExecutionContextExpression Class</a><br /><a href="N_ConnectQl_Expressions">ConnectQl.Expressions Namespace</a><br />