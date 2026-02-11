package com.unifiednews.blogaggregator.model;

import java.io.Serializable;
import java.util.Date;

public class BlogArticle implements Serializable {
    private String title;
    private String link;
    private String description;
    private String author;
    private Date publishedDate;
    private String source;

    public BlogArticle() {}

    public BlogArticle(String title, String link, String description, String author, Date publishedDate, String source) {
        this.title = title;
        this.link = link;
        this.description = description;
        this.author = author;
        this.publishedDate = publishedDate;
        this.source = source;
    }

    // Getters and Setters
    public String getTitle() { return title; }
    public void setTitle(String title) { this.title = title; }

    public String getLink() { return link; }
    public void setLink(String link) { this.link = link; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public String getAuthor() { return author; }
    public void setAuthor(String author) { this.author = author; }

    public Date getPublishedDate() { return publishedDate; }
    public void setPublishedDate(Date publishedDate) { this.publishedDate = publishedDate; }

    public String getSource() { return source; }
    public void setSource(String source) { this.source = source; }
}
