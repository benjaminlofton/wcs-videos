package ban.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(value = HttpStatus.NOT_FOUND, reason="This resource does not exist...")
public class ResourceNotFoundException extends RuntimeException {}
