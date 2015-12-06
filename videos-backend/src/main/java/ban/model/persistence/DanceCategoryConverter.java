package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMarshaller;

/**
 * Created by bnorrish on 11/16/15.
 */
public class DanceCategoryConverter implements DynamoDBMarshaller<DanceCategory> {

  @Override
  public String marshall(DanceCategory value) {
    return value.toString();
  }

  @Override
  public DanceCategory unmarshall(Class<DanceCategory> clazz, String value) {
    return DanceCategory.valueOf(value);
  }

}
