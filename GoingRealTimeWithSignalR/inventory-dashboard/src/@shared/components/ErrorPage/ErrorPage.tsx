import { map } from 'lodash';
import React from 'react';

import { ErrorPageConfig } from './errorPageConfigs';
import { useStyles } from './ErrorPage.style';
import { Button } from '@material-ui/core';

interface ErrorPageProps {
  config: ErrorPageConfig;
}

export const ErrorPage: React.FC<ErrorPageProps> = ({
  config: { primaryTitle, secondaryTitle, subTitle, description, buttons },
}) => {
  const s = useStyles();

  return (
    <div className={s.wrapper}>
      <div className={s.content}>
        <div>
          <h1>
            {primaryTitle && <p className={s.primaryTitle}>{primaryTitle}</p>}
            {secondaryTitle && <p className={s.secondaryTitle}>{secondaryTitle}</p>}
          </h1>
          <h2 className={s.subTitle}>{subTitle}</h2>
          <div className={s.description}>
            {map(description, (text, i) => <p key={i}>{text}</p>)}
          </div>
          <div className={s.buttons}>
            {map(buttons, (props, i) => <Button key={i} {...props} />)}
          </div>
        </div>
      </div>
      <div className={s.footer}/>
    </div>
  );
};

ErrorPage.displayName = 'ErrorPage';
