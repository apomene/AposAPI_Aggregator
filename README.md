
# API Aggregator – .NET 9 Web API

This API Aggregator is a .NET 9 Web API service that consolidates data from multiple external sources into a single, unified endpoint. It currently supports integration with public APIs such as OpenWeatherMap and NewsAPI, and is designed to be easily extendable to additional APIs. The system allows clients to retrieve aggregated data filtered by category or keywords, while optimizing performance using parallel execution, in-memory caching, and request statistics tracking. Each API client is pluggable, and the architecture supports robust error handling  and detailed logging usning NLOG.

In addition to data aggregation, the API exposes a statistics endpoint that reports the number of requests and average response times for each external API, grouped into performance buckets (fast, average, slow).
