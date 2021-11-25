import React, {useEffect} from 'react';
import Image from "../Image/Image";
import cl from "./ImageList.module.css";
import ReactTooltip from "react-tooltip";

const ImageList = ({imageList, tooltipId}) => {
    useEffect(() => {
        ReactTooltip.rebuild();
    }, [imageList]);

    return (
        <div className={cl.imageList}>

            {imageList.map(img =>
                <Image image={img} tooltipId={tooltipId} key={img.id}/>
            )}

        </div>
    );
};

export default ImageList;