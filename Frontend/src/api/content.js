import axios from './axios';

export const searchContent = async (query) => {
    try {
        const response = await axios.get(`/content/search?q=${query}`);
        return response.data;
    } catch (error) {
        console.error("Error searching content:", error);
        throw error;
    }
};
