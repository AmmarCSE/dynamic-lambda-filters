dynamic-lambda-filters
====================================================
dynamic-lambda-filters is a LINQ utility module which generates *smart* LINQ expressions on the fly by inspecting the injected model.
This utility is a good back-end solution for facilitating responsive filters on the client side. 

A *smart* filter is defined as only displaying *active* options that will gaurantee a filtered sub-set if selected.
To illustrate, imagine two sets of checkbox filters:
```
Hotels:

[] Radisson Blu
[] Marriot
[] Holiday Inn

Cities:

[] Las Angeles
[] San Diego
[] San Francisco
```
and the Marriot only has branches in Las Angeles or San Francisco, then only those option will appear of the Marriot option is selected.
```
Hotels:

[] Radisson Blu
[X] Marriot
[] Holiday Inn

Cities:

[] Las Angeles
[] San Francisco
```

It can be chosen which is the *control* group(hotels in the above example) that drives the filters. The result is that **all** hotel filters will always display while the secondary filters are updated based on the options selected in the *control* group.
