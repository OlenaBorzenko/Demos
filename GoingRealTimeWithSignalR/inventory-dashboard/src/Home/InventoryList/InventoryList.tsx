import { observer } from 'mobx-react';
import React  from 'react';
import { CustomCard } from '@shared/components/Card';
import { CardConfig } from '@shared/components/Card/CardConfig';
import { IArticleCheckpointModel } from 'Home/store/ArticleCheckpointModel';
import { IStorageLocationModel } from 'Home/store/StorageLocationModel';
import _ from 'lodash';
import { EmptyScreen } from '@shared/components';
import { Typography } from '@material-ui/core';

interface Props {
  locationsList: IStorageLocationModel[];
}

export const InventoryList: React.FC<Props> = observer(props => {
  const { locationsList } = props;

  const cardConfig:Array<CardConfig> = [
    {
      name: 'articleName',
      label: 'Article Name',
      content: (checkpoint: IArticleCheckpointModel) => {
        return (
          <Typography
            gutterBottom
            align={'center'}
            color="textPrimary">
            {checkpoint.articleName}:
          </Typography>
        )
      },
    },
    {
      name: 'quantity',
      label: 'Quantity',
      content: (checkpoint: IArticleCheckpointModel) => {
        return (
          <Typography
            align={'center'}
            variant={'h6'}
            color={'textPrimary'}>
            {checkpoint.quantity}
          </Typography>
        )
      },
    },
  ];

  return (
    <>
      {!_.isEmpty(locationsList) ?
        <>
          {locationsList.map((item: IStorageLocationModel, index: number) => (
            <CustomCard
              key={`${name}-${index}`}
              cardConfig={cardConfig}
              item={item} />
          ))}
        </> :
        <EmptyScreen type='error'/>
      }
    </>
  );
});
