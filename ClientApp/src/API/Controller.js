import axios from "axios";

export default class Controller {
    static async getAllImagesFromPixabay(query) {
        return await axios.get(
            'https://pixabay.com/api/',
            {params: {q: query, pretty: true, image_type: 'photo', key: '1923807-a9f36f13c40dea26ff0b06414'}}
        ).then(
            response => {
                return response.data.hits.map(item => {
                    return {id: item.id, title: item.user, tags: item.tags, previewURL:item.previewURL, pageURL:item.pageURL};
                })
            }
        );
    }

    static async getAllImagesFromFlickr(query) {
        return await axios.get('https://api.flickr.com/services/feeds/photos_public.gne',
            {baseURL:'', params: {tags: query, format: 'json', nojsoncallback: 1}}
        ).then(
            response => {
                debugger
                return response.data.items.map(item => {
                    return {id: item.id, title: item.title, tags: item.tags, previewURL:item.previewURL, pageURL:item.link};
                })
            }
        );
    }
};
