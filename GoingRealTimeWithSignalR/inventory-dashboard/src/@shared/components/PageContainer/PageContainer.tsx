import classNames from 'classnames';
import React, { HTMLAttributes } from 'react';
import { useStyles } from './PageContainer.style';

interface Props extends HTMLAttributes<HTMLDivElement> {}

export const PageContainer: React.FC<Props> = ({ className, children, ...divProps }) => {
  const s = useStyles();

  return (
    <div {...divProps} className={classNames(s.container, s.centeredContent, className)}>
      {children}
    </div>
  );
};

PageContainer.displayName = 'PageContainer';
