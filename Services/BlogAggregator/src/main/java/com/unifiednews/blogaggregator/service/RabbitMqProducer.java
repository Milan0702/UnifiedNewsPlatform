package com.unifiednews.blogaggregator.service;

import com.unifiednews.blogaggregator.model.BlogArticle;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class RabbitMqProducer {

    @Autowired
    private RabbitTemplate rabbitTemplate;

    private static final String QUEUE_NAME = "blogs.raw";

    public void publishBlog(BlogArticle article) {
        rabbitTemplate.convertAndSend(QUEUE_NAME, article);
        System.out.println("Published blog: " + article.getTitle());
    }
}
