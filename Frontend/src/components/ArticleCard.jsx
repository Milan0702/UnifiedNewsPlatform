const ArticleCard = ({ article }) => {
    return (
        <div className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300">
            {article.urlToImage && (
                <img className="h-48 w-full object-cover" src={article.urlToImage} alt={article.title} />
            )}
            <div className="p-6">
                <div className="flex items-center justify-between mb-2">
                    <span className="text-xs font-semibold text-indigo-600 uppercase tracking-wider">{article.category}</span>
                    <span className="text-xs text-gray-500">{new Date(article.publishedAt).toLocaleDateString()}</span>
                </div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2 line-clamp-2">
                    <a href={article.url} target="_blank" rel="noopener noreferrer" className="hover:text-indigo-600">
                        {article.title}
                    </a>
                </h3>
                <p className="text-gray-600 text-sm mb-4 line-clamp-3">{article.description}</p>
                <div className="flex items-center justify-between">
                    <span className="text-xs text-gray-500">{article.source}</span>
                    <a href={article.url} target="_blank" rel="noopener noreferrer" className="text-indigo-600 hover:text-indigo-800 text-sm font-medium">
                        Read more &rarr;
                    </a>
                </div>
            </div>
        </div>
    );
};

export default ArticleCard;
