# AsyncEnumerableExtensions.Join Method 
 


## Overload List
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Join__4">Join(TLeft, TRight, TKey, TResult)(IAsyncEnumerable(TLeft), IAsyncEnumerable(TRight), Func(TLeft, TKey), Func(TRight, TKey), Func(TLeft, TRight, TResult), IComparer(TKey))</a></td><td>
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Join__4_1">Join(TLeft, TRight, TKey, TResult)(IAsyncEnumerable(TLeft), IAsyncEnumerable(TRight), Func(TLeft, TKey), ExpressionType, Func(TRight, TKey), Func(TLeft, TRight, Boolean), Func(TLeft, TRight, TResult), IComparer(TKey))</a></td><td>
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*.</td></tr></table>&nbsp;
<a href="#asyncenumerableextensions.join-method">Back to Top</a>

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />