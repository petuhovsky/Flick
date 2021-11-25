import React, {useRef} from 'react';
import cl from "./Image.module.css";

const Image = ({image, tooltipId}) => {
    const tagsElement = useRef();

    const navigateToImageURL = () => {
        window.location.href = image.pageURL;
    }

    const showTags = () => {
        tagsElement.current.style.opacity = '1';
        return true;
    }

    const hideTags = () => {
        tagsElement.current.style.opacity = '0';
        return true;
    }

    return (
        <div className={cl.image_div}>
            <img className={cl.image} data-for={tooltipId} data-tip={image.title} src={image.previewURL}
                 onMouseEnter={showTags} onMouseLeave={hideTags}
                 onClick={navigateToImageURL}
            />
            <div ref={tagsElement} data-for={tooltipId} data-tip={image.title} className={cl.image_tags}
                onMouseEnter={showTags} onMouseLeave={hideTags}
            >
                {image.tags}
            </div>
        </div>
    );
};

export default Image;
