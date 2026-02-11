package com.unifiednews.blogaggregator;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class BlogAggregatorApplication {

	public static void main(String[] args) {
		SpringApplication.run(BlogAggregatorApplication.class, args);
	}

}
