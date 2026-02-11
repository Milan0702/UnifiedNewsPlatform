package com.unifiednews.blogaggregator.service;

import com.rometools.rome.feed.synd.SyndEntry;
import com.rometools.rome.feed.synd.SyndFeed;
import com.rometools.rome.io.SyndFeedInput;
import com.rometools.rome.io.XmlReader;
import com.unifiednews.blogaggregator.model.BlogArticle;
import io.github.resilience4j.circuitbreaker.annotation.CircuitBreaker;
import io.github.resilience4j.retry.annotation.Retry;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.net.URL;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

@Service
public class RssFetcherService {

    @Autowired
    private RabbitMqProducer producer;

    private static final String RSS_URL = "https://medium.com/feed/tag/technology"; // Example Feed

    @Scheduled(fixedRate = 600000) // 10 minutes
    @Retry(name = "rssFetcher")
    @CircuitBreaker(name = "rssFetcher", fallbackMethod = "fallbackFetch")
    public void fetchRssFeed() {
        System.out.println("Fetching RSS feed...");
        try {
            URL feedUrl = new URL(RSS_URL);
            SyndFeedInput input = new SyndFeedInput();
            SyndFeed feed = input.build(new XmlReader(feedUrl));

            for (SyndEntry entry : feed.getEntries()) {
                BlogArticle article = new BlogArticle(
                        entry.getTitle(),
                        entry.getLink(),
                        entry.getDescription() != null ? entry.getDescription().getValue() : "",
                        entry.getAuthor(),
                        entry.getPublishedDate() != null ? entry.getPublishedDate() : new Date(),
                        "Medium/Tech"
                );
                producer.publishBlog(article);
            }
        } catch (Exception e) {
            throw new RuntimeException("Failed to fetch RSS feed", e);
        }
    }

    public void fallbackFetch(Exception e) {
        System.err.println("RSS Fetch failed. Circuit Breaker open or Retries exhausted: " + e.getMessage());
    }
}
