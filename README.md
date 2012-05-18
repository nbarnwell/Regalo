What is it?
===========
A simple event sourcing "framework" based on Greg Young's work in DDD, CQRS and Event Sourcing. In truth, as you'll see, it's barely a "framework" as much of a bunch of reusable code. :)

Why'd you call it "Regalo"?
===========================
Well it's an event sourcing framework, and events tell a story. You might "regale" someone with a story, and I just swopped the trailing "e" for an "o" to make it sound cool. Hence "Regalo". I pronounce it "regarlo", in case you're wondering.

How do I use it?
================
Regalo comes in two significant parts - there's _Regalo.Core_ itself. This provides a bunch of interfaces and a Greg Young-inspired `AggregateRoot` class to derive your... aggregate roots from. You also get an event sourcing repository implementation. What's missing from that picture is the actual persistence. That's where _Regalo.RavenDB_ (and at some point _Regalo.SqlServer_) come in. They provide an event store implementation.

Getting started is a case of installing one of the event store implementation packages via nuget.org e.g. `install-package regalo.ravendb`, then configuring a few dependencies (all Regalo libraries rely on the Dependency Inversion principle).

I'll try to build a "getting started" page on the wiki asap. In the meantime, check out http://github.com/nbarnwell/vita. This is and will continue to be the sample implementation to demonstrate Regalo usage.